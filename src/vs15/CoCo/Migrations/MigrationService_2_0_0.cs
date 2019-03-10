using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Media;
using System.Xml;
using System.Xml.XPath;
using CoCo.Analyser;
using CoCo.Providers;
using CoCo.Settings;
using CoCo.Utils;

namespace CoCo
{
    /// <summary>
    /// Service that migrates existing options to a new versions
    /// </summary>
    public partial class MigrationService
    {
        /// <summary>
        /// VS seaves the default background as #FF010000 for light and dark theme.
        /// </summary>
        /// <remarks>
        /// Unfortunatly, if background was set manually at #FF010000 this change will be lost
        /// </remarks>
        private static readonly Color _defaultBackground = Color.FromRgb(1, 0, 0);

        /// <summary>
        /// VS seaves the default foreground as #FF000000 for light and dark theme.
        /// </summary>
        /// <remarks>
        /// Unfortunatly, if background was set manually at #FF000000 this change will be lost
        /// </remarks>
        private static readonly Color _defaultForeground = Colors.Black;

        /// <remarks>
        /// The location of current vssettings is unknown, as workaround, invoke export command to the temp file,
        /// parse an all needed info and remove the temp file.
        /// </remarks>
        private static readonly string _tempCurrenSettings = Path.Combine(Path.GetTempPath(), "coco_temp_current_settings.vssettings");

        /// <summary>
        /// Try to migrate an existing CoCo classifications from the previous version of vsix to the current format
        /// </summary>
        /// <remarks>
        /// <see cref="Microsoft.VisualStudio.Shell.Interop.IVsFontAndColorStorage"/> can retreive only applied and changed Display Items =>
        /// it cannot be used to retrieve the old CoCo classifications, because they aren't actual => Try to retrieve they directly
        /// from current vssettings, and find usages of old CoCo classifications
        /// </remarks>
        public static void MigrateSettingsTo_2_0_0()
        {
            var cocoSettings = new FileInfo(Paths.CoCoSettingsFile);

            // NOTE: create CoCo folder if it doesn't exist
            if (!cocoSettings.Directory.Exists)
            {
                cocoSettings.Directory.Create();
            }

            // NOTE: assumes that if the configuration file exists, the migration is complete
            if (cocoSettings.Exists || ServicesProvider.Instance.DTE is null) return;

            /// NOTE: <see cref="Microsoft.VisualStudio.Shell.Interop.IVsCommandWindow.ExecuteCommand"/> just posts message
            /// of execution command to shell's queue, that doesn't match the current synchronous model,
            /// <see cref="EnvDTE._DTE.ExecuteCommand"/> aslo is asynchronous, but more usefull, if use it together with a busy-looping
            /// (<see cref="EnvDTE._dispCommandEvents_Event.AfterExecute"/> doesn't solve the issue)
            ServicesProvider.Instance.DTE.ExecuteCommand("Tools.ImportandExportSettings", $"/export:\"{_tempCurrenSettings}\"");
            if (!IsExportDone(60)) return;

            var tryToDelete = true;
            XmlNodeList existingSettings;
            try
            {
                var document = new XmlDocument();
                document.Load(_tempCurrenSettings);
                existingSettings =
                    document.SelectNodes("//Category[@name = 'Environment_FontsAndColors']//Item[starts-with(@Name, 'CoCo')]");
            }
            catch (XPathException)
            {
                return;
            }
            catch (FileNotFoundException)
            {
                tryToDelete = false;
                return;
            }
            catch (IOException)
            {
                tryToDelete = false;
                return;
            }
            finally
            {
                if (tryToDelete)
                {
                    File.Delete(_tempCurrenSettings);
                }
            }

            MigateSettings(existingSettings);
        }

        private static void MigateSettings(XmlNodeList existingSettings)
        {
            var result = new Dictionary<string, ClassificationSettings>();
            for (int i = 0; i < existingSettings.Count; ++i)
            {
                var item = existingSettings[i];
                var name = item.Attributes["Name"]?.Value;
                if (name is null || result.ContainsKey(name)) continue;

                var classificationSettings = new ClassificationSettings();
                classificationSettings.Name = name;

                var value = item.Attributes["BoldFont"]?.Value;
                if (!(value is null))
                {
                    classificationSettings.IsBold = value.EqualsNoCase("Yes");
                }

                value = item.Attributes["Foreground"]?.Value;
                // NOTE: doesn't save the color that equals a default value, because it will be correctly set from the default formatting
                if (TryParseColor(value, out var foreground) && !foreground.Equals(_defaultForeground))
                {
                    classificationSettings.Foreground = foreground;
                }

                value = item.Attributes["Background"]?.Value;
                // NOTE: doesn't save the color that equals a default value, because it will be correctly set from the default formatting
                if (TryParseColor(value, out var background) && !background.Equals(_defaultBackground))
                {
                    classificationSettings.Background = background;
                }

                if (classificationSettings.IsBold.HasValue ||
                    classificationSettings.Foreground.HasValue ||
                    classificationSettings.Background.HasValue)
                {
                    // NOTE: add if and only if a some of values are presented
                    result.Add(classificationSettings.Name, classificationSettings);
                }
            }

            var settings = new EditorSettings
            {
                Languages = new List<EditorLanguageSettings>
                {
                    new EditorLanguageSettings
                    {
                        Name = Languages.CSharp,
                        CurrentClassifications = result.Values.ToList(),
                        Presets = new List<PresetSettings>(),
                    }
                }
            };

            var allSettings = new Settings.Settings
            {
                Editor = settings,
                EditorPath = Paths.CoCoSettingsFile,

                QuickInfo = new QuickInfoSettings
                {
                    Languages = new List<QuickInfoLanguageSettings>(),
                },
                QuickInfoPath = Paths.CoCoQuickInfoSettingsFile,
            };
            SettingsManager.SaveSettings(allSettings);
        }

        /// <summary>
        /// Busy-looping that just check if the <see cref="_tempCurrenSettings"/> is locked or free.
        /// <para>It will check not more than <paramref name="repeatCount"/> times</para>
        /// </summary>
        private static bool IsExportDone(int repeatCount)
        {
            var currentCount = 0;
            while (currentCount < repeatCount)
            {
                try
                {
                    using (var stream = File.OpenText(_tempCurrenSettings))
                        return true;
                }
                catch (IOException)
                {
                    using (var waitHandle = new ManualResetEventSlim(false))
                    {
                        waitHandle.Wait(100);
                    }
                    ++currentCount;
                }
            }
            return false;
        }

        private static bool TryParseColor(string value, out Color color)
        {
            byte ToByte(int integer, int offset) => (byte)(integer >> offset & 0xFF);

            // NOTE: 0xABGR – 10 chars
            if (!(value is null) && value.Length == 10)
            {
                value = value.Substring(4);
                if (int.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var res))
                {
                    // NOTE: colors are stored in the vssettings as ABGR, and A is 0, avoid usages of it
                    color = Color.FromRgb(ToByte(res, 0), ToByte(res, 8), ToByte(res, 16));
                    return true;
                }
            }
            color = new Color();
            return false;
        }
    }
}