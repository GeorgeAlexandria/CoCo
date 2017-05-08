using NLog;
using NLog.Config;
using NLog.Targets;

namespace CoCo
{
    internal static class NLog
    {
        internal static void Initialize()
        {
            LoggingConfiguration config = new LoggingConfiguration();
            FileTarget fileTarget = new FileTarget("File");
            FileTarget fileDebugTarget = new FileTarget("File debug");

            fileTarget.Layout = "${date}___${level}___${message}";
            fileTarget.FileName = @"${nlogdir}\file.log";

            fileDebugTarget.Layout = "${date}___${level}___${message}${newline}${stacktrace}";
            fileDebugTarget.FileName = @"${nlogdir}\file_debug.log";

            config.AddTarget(fileTarget);
            config.AddTarget(fileDebugTarget);
            config.AddRule(LogLevel.Debug, LogLevel.Debug, fileDebugTarget, "*");
            config.AddRule(LogLevel.Info, LogLevel.Fatal, fileTarget, "*");

            //LogManager.ThrowConfigExceptions = true;
            //LogManager.ThrowExceptions = true;
            LogManager.Configuration = config;
        }
    }
}