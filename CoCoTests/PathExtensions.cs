using System;
using System.IO;

namespace CoCoTests
{
    internal static class PathExtensions
    {
        /// <summary>
        /// Returns the absolute path for the <paramref name="path"/> using <paramref name="rootPath"/>
        /// </summary>
        /// <remarks>
        /// Returns original <paramref name="path"/> if it's the asbolute path, else combines it with <paramref name="rootPath"/>
        /// </remarks>
        public static string GetFullPath(this string path, string rootPath) =>
            string.Equals(Path.GetFullPath(path), path, StringComparison.Ordinal)
                ? path
                : Path.GetFullPath(Path.Combine(rootPath, path));

        /// <summary>
        /// Returns the directory information for the <paramref name="path"/>
        /// </summary>
        public static string GetDirectoryName(this string path) => Path.GetDirectoryName(path);

        public static bool IsTrue(this string name) => "true".Equals(name, StringComparison.OrdinalIgnoreCase);
    }
}