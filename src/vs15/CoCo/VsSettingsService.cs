using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Media;
using System.Xml;
using System.Xml.XPath;
using CoCo.Settings;
using CoCo.Utils;

namespace CoCo
{
    public static class VsSettingsService
    {
        /// <remarks>
        /// The location of current vssettings is unknown, as workaround, invoke export command to the temp file,
        /// parse an all needed info, return they and remove the temp file.
        /// </remarks>
        private static readonly string _tempCurrenSettings = Path.Combine(Path.GetTempPath(), "coco_temp_current_settings.vssettings");

        /// <summary>
        /// Get existing CoCo classifications from the previous version of vsix
        /// </summary>
        /// <remarks>
        /// <see cref="Microsoft.VisualStudio.Shell.Interop.IVsFontAndColorStorage"/> can retreive only applied and changed Display Items =>
        /// it cannot be used to retrieve the old CoCo classifications, because they aren't actual => Try to retrieve they directly
        /// from current vssettings, and find usages of old CoCo classifications
        /// </remarks>
        public static Dictionary<string, ClassificationSettings> GetExistingClassifications()
        {
            bool TryParseColor(string value, out Color color)
            {
                byte ToByte(int integer, int offset) => (byte)(integer >> offset & 0xFF);

                // NOTE: 0xABGR – 10 chars
                if (!(value is null) && value.Length == 10)
                {
                    value = value.Substring(2);
                    if (int.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var res))
                    {
                        // NOTE: colors are stored in the vssettings as ABGR
                        color = Color.FromArgb(ToByte(res, 24), ToByte(res, 0), ToByte(res, 8), ToByte(res, 16));
                        return true;
                    }
                }
                color = new Color();
                return false;
            }

            var result = new Dictionary<string, ClassificationSettings>();

            /// NOTE: <see cref="Microsoft.VisualStudio.Shell.Interop.IVsCommandWindow.ExecuteCommand"/> just posts message
            /// of execution command to shell's queue, that doesn't match the current synchronous model,
            /// <see cref="EnvDTE._DTE.ExecuteCommand"/> aslo is asynchronous, but more usefull, if use it together with a busy-looping
            /// (<see cref="EnvDTE._dispCommandEvents_Event.AfterExecute"/> doesn't solve the issue)
            ClassificationManager.Instance.DTE.ExecuteCommand("Tools.ImportandExportSettings", $"/export:\"{_tempCurrenSettings}\"");
            if (!IsExportDone(30)) return result;

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
                File.Delete(_tempCurrenSettings);
                return result;
            }
            catch (FileNotFoundException)
            {
                return result;
            }

            for (int i = 0; i < existingSettings.Count; ++i)
            {
                var item = existingSettings[i];
                var name = item.Attributes["Name"]?.Value;
                if (name is null || result.ContainsKey(name)) continue;

                var settings = new ClassificationSettings();
                settings.Name = name;

                var value = item.Attributes["BoldFont"]?.Value;
                if (!(value is null))
                {
                    settings.IsBold = value.EqualsNoCase("Yes");
                }

                value = item.Attributes["Foreground"]?.Value;
                if (TryParseColor(value, out var foreground))
                {
                    settings.Foreground = foreground;
                }

                value = item.Attributes["Background"]?.Value;
                if (TryParseColor(value, out var background))
                {
                    settings.Background = background;
                }

                if (settings.IsBold.HasValue || settings.Foreground.HasValue || settings.Background.HasValue)
                {
                    // NOTE: add if and only if a some of values are presented
                    result.Add(settings.Name, settings);
                }
            }

            File.Delete(_tempCurrenSettings);
            return result;
        }

        /// <summary>
        /// Busy-looping that just check if the <see cref="_tempCurrenSettings"/> is locked or free.
        /// <para>It will check not more than <paramref name="repeatCount"/> times</para>
        /// </summary>
        private static bool IsExportDone(int repeatCount)
        {
            while (repeatCount < 30)
            {
                try
                {
                    using (var stream = File.OpenText(_tempCurrenSettings))
                    {
                    }
                    break;
                }
                catch (IOException)
                {
                    using (var waitHandle = new ManualResetEventSlim(false))
                    {
                        waitHandle.Wait(100);
                    }
                    ++repeatCount;
                }
            }

            return repeatCount < 30;
        }
    }
}