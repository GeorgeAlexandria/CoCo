using System.Diagnostics;

#if DEBUG

using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;

#endif

// TODO: remove NLog reference for RELEASE mode from .csproj
namespace CoCo
{
    /// <summary>
    /// Now <see cref="Log"/> is used only in the debug configuration, <see cref="ConditionalAttribute"/>
    /// and Preprocessor Directives are used for it.
    /// </summary>
    internal static class Log
    {
#if DEBUG
        private static Logger _logger;
#endif

        private const string _debug = "DEBUG";

        static Log()
        {
#if DEBUG
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

            BufferingTargetWrapper bufferWrapper = new BufferingTargetWrapper
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

            _logger = LogManager.GetLogger(nameof(_logger));
#endif
        }

        // NOTE: it's tiny optimization for string building
        [Conditional(_debug)]
        internal static void Debug<TArg>(string message, TArg arg)
        {
#if DEBUG
            _logger.Info(message, arg);
#endif
        }

        [Conditional(_debug)]
        internal static void Debug<TArg1, TArg2>(string message, TArg1 arg1, TArg2 arg2)
        {
#if DEBUG
            _logger.Info(message, arg1, arg2);
#endif
        }

        [Conditional(_debug)]
        internal static void Debug<TArg1, TArg2, TArg3>(string message, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
#if DEBUG
            _logger.Info(message, arg1, arg2, arg3);
#endif
        }
    }
}