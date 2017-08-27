using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;

// TODO: remove NLog reference for RELEASE mode from .csproj
namespace CoCo
{
    internal static class NLog
    {
        private const string _debug = "DEBUG";

        // NOTE: Config initialization can be extracted to nlog config file
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

            const string format = "${logger}:_${date}:_${level}:_${message}";
            fileTarget.Layout = format;
            fileTarget.FileName = $"{appDataLocal}\\file.log";

            fileDebugTarget.Layout = $"{format}${{newline}}${{stacktrace}}";
            fileDebugTarget.FileName = $"{appDataLocal}\\file_debug.log";

            BufferingTargetWrapper bufferWrapper = new BufferingTargetWrapper()
            {
                WrappedTarget = fileTarget,
                BufferSize = 300,
                FlushTimeout = 10,
                Name = "Buffered file",
                OptimizeBufferReuse = true
            };

            LoggingConfiguration config = new LoggingConfiguration();
            config.AddTarget(bufferWrapper);
            config.AddTarget(fileDebugTarget);
            config.AddRule(LogLevel.Debug, LogLevel.Debug, fileDebugTarget, "*");
            config.AddRule(LogLevel.Info, LogLevel.Fatal, bufferWrapper, "*");

            //LogManager.ThrowConfigExceptions = true;
            //LogManager.ThrowExceptions = true;
            LogManager.Configuration = config;
        }

        // NOTE: it's tiny optimization for string building
        [Conditional(_debug)]
        internal static void ConditionalInfo<TArg>(this Logger logger, [Localizable(false)] string message, TArg arg) =>
            logger.Info(message, arg);

        [Conditional(_debug)]
        internal static void ConditionalInfo<TArg1, TArg2>(this Logger logger, [Localizable(false)] string message, TArg1 arg1, TArg2 arg2) =>
            logger.Info(message, arg1, arg2);

        [Conditional(_debug)]
        internal static void ConditionalInfo<TArg1, TArg2, TArg3>(this Logger logger, [Localizable(false)] string message, TArg1 arg1, TArg2 arg2, TArg3 arg3) =>
            logger.Info(message, arg1, arg2, arg3);
    }
}