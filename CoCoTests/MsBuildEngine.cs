using System;
using System.Collections;
using CoCo;
using Microsoft.Build.Framework;

namespace CoCoTests
{
    // TODO: doesn't log error|and warning as debug
    public class MsBuildEngine : IBuildEngine
    {
        public bool ContinueOnError => false;

        public int LineNumberOfTaskNode => 0;

        public int ColumnNumberOfTaskNode => 0;

        public string ProjectFileOfTaskNode => "";

        public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs) => throw new NotImplementedException();

        public void LogCustomEvent(CustomBuildEventArgs e)
        {
            Log.Debug<int>(e.Message);
        }

        public void LogErrorEvent(BuildErrorEventArgs e)
        {
            Log.Debug<int>(e.Message);
        }

        public void LogMessageEvent(BuildMessageEventArgs e)
        {
            Log.Debug<int>(e.Message);
        }

        public void LogWarningEvent(BuildWarningEventArgs e)
        {
            Log.Debug<int>(e.Message);
        }
    }
}