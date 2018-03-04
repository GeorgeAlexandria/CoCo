using System;
using System.Collections;
using Microsoft.Build.Framework;

namespace CoCoTests
{
    public sealed class MsBuildEngine : IBuildEngine
    {
        private CoCoLog.Logger _logger;

        public MsBuildEngine(CoCoLog.Logger logger)
        {
            _logger = logger;
        }

        public bool ContinueOnError => false;

        public int LineNumberOfTaskNode => 0;

        public int ColumnNumberOfTaskNode => 0;

        public string ProjectFileOfTaskNode => "";

        public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs) =>
            throw new NotImplementedException();

        public void LogCustomEvent(CustomBuildEventArgs e)
        {
            _logger.Debug(e.Message);
        }

        public void LogErrorEvent(BuildErrorEventArgs e)
        {
            _logger.Error(e.Message);
        }

        public void LogMessageEvent(BuildMessageEventArgs e)
        {
            _logger.Info(e.Message);
        }

        public void LogWarningEvent(BuildWarningEventArgs e)
        {
            _logger.Warn(e.Message);
        }
    }
}