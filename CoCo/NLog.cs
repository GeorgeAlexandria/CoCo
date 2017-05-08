using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace CoCo
{
    internal static class NLog
    {
        internal static void Initialize()
        {
            string appDataLocal = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\CoCo";
            if (!Directory.Exists(appDataLocal))
            {
                Directory.CreateDirectory(appDataLocal);
            }

            LoggingConfiguration config = new LoggingConfiguration();
            FileTarget fileTarget = new FileTarget("File");
            FileTarget fileDebugTarget = new FileTarget("File debug");

            fileTarget.Layout = "${date}___${level}___${message}";
            fileTarget.FileName = $"{appDataLocal}\\file.log";

            fileDebugTarget.Layout = "${date}___${level}___${message}${newline}${stacktrace}";
            fileDebugTarget.FileName = $"{appDataLocal}\\file_debug.log";

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