using System;
using System.IO;

namespace CoCo
{
    public static class Paths
    {
        public static string CoCoFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CoCo");

        public static string CoCoSettingsFile = Path.Combine(CoCoFolder, "CoCo.config");
    }
}