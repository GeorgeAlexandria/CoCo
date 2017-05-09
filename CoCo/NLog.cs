using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

// TODO: remove NLog reference for RELEASE mode from .csproj
namespace CoCo
{
    internal static class NLog
    {
        private const string _debug = "DEBUG";

        [Conditional(_debug)]
        internal static void Initialize()
        {
            string appDataLocal = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\CoCo";
            if (!Directory.Exists(appDataLocal))
            {
                Directory.CreateDirectory(appDataLocal);
            }

            FileTarget fileTarget = new FileTarget("File");
            FileTarget fileDebugTarget = new FileTarget("File debug");

            fileTarget.Layout = "${date}___${level}___${message}";
            fileTarget.FileName = $"{appDataLocal}\\file.log";

            fileDebugTarget.Layout = "${date}___${level}___${message}${newline}${stacktrace}";
            fileDebugTarget.FileName = $"{appDataLocal}\\file_debug.log";

            LoggingConfiguration config = new LoggingConfiguration();
            config.AddTarget(fileTarget);
            config.AddTarget(fileDebugTarget);
            config.AddRule(LogLevel.Debug, LogLevel.Debug, fileDebugTarget, "*");
            config.AddRule(LogLevel.Info, LogLevel.Fatal, fileTarget, "*");

            //LogManager.ThrowConfigExceptions = true;
            //LogManager.ThrowExceptions = true;
            LogManager.Configuration = config;
        }

        [Conditional(_debug)]
        internal static void ConditionalInfo<TArg1, TArg2>(this Logger logger, [Localizable(false)] string message, TArg1 arg1, TArg2 arg2) =>
            logger.Info(message, arg1, arg2);
    }
}