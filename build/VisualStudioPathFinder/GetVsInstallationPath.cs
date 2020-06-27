using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.VisualStudio.Setup.Configuration;
using Microsoft.Win32;

namespace VisualStudioPathFinder
{
    public class GetVsInstallationPath : Task
    {
        [Required]
        public string VsMajorVersion { get; set; }

        [Output]
        public string InstallationPath { get; set; }

        public override bool Execute()
        {
            if (VsMajorVersion == "14")
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\VisualStudio\14.0"))
                {
                    if (key != null)
                    {
                        var obj = key.GetValue("InstallDir");
                        if (obj is string dir)
                        {
                            InstallationPath = Path.Combine(dir, "devenv.exe");
                            return true;
                        }
                    }
                }

                Log.LogError("Could not find visual studio installation path for {0}", VsMajorVersion);
                return true;
            }

            if (new SetupConfiguration() is ISetupConfiguration2 setupConfiguration)
            {
                var allInstances = setupConfiguration.EnumAllInstances();
                var array = new ISetupInstance[1];
                allInstances.Next(1, array, out var takenCount);
                while (takenCount == 1)
                {
                    if (array[0] is ISetupInstance2 setupInstance && setupInstance.IsLaunchable())
                    {
                        if (setupInstance.GetInstallationVersion().StartsWith(VsMajorVersion))
                        {
                            InstallationPath = Path.Combine(setupInstance.GetInstallationPath(), setupInstance.GetProductPath());
                            return true;
                        }
                    }
                    allInstances.Next(1, array, out takenCount);
                }
            }

            Log.LogError("Could not find visual studio installation path for {0}", VsMajorVersion);
            return true;
        }
    }
}