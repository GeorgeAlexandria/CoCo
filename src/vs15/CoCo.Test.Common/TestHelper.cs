using System.IO;
using System.Runtime.CompilerServices;
using CoCo.Utils;

namespace CoCo.Test.Common
{
    public static class TestHelper
    {
        internal static string GetPathRelativeToTest(string projectPath) => GetPathRelativeToThis(projectPath);

        private static string GetPathRelativeToThis(string path, [CallerFilePath] string sourceCallerPath = null)
        {
            // NOTE: ../../../CoCo.Test.Common/TestHelper.cs
            var sourceDirectory = sourceCallerPath.GetDirectoryName().GetDirectoryName().GetDirectoryName().GetDirectoryName();
            return Path.Combine(sourceDirectory, path);
        }
    }
}