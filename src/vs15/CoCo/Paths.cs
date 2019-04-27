using System;
using System.IO;

namespace CoCo
{
    public static class Paths
    {
        public static string CoCoFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CoCo");

        public static string CoCoClassificationSettingsFile = Path.Combine(CoCoFolder, "CoCo classifications.config");

        public static string CoCoGeneralSettingsFile = Path.Combine(CoCoFolder, "CoCo general.config");
    }
}