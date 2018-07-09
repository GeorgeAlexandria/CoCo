using System;
using System.IO;

namespace CoCo
{
    public static class PathsManager
    {
        public static string CoCoFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CoCo");

        public static string CoCoSettingsFile = Path.Combine(CoCoFolder, "CoCo.config");

        /// <summary>
        /// Creates CoCo directory if it doesn't exist
        /// </summary>
        public static void Initialize()
        {
            var cocoFolder = new DirectoryInfo(CoCoFolder);

            // NOTE: create CoCo folder if it doesn't exist
            if (!cocoFolder.Exists)
            {
                cocoFolder.Create();
            }
        }
    }
}