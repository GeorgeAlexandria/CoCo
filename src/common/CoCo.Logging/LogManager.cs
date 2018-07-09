using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace CoCo.Logging
{
    // TODO: use pool of objects
    public static class LogManager
    {
        // TODO: duplicated path %LocalApplicationData%/CoCo
        private static string _logsFolder =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CoCo/Logs");

        public static Logger GetLogger(string name)
        {
            var directory = new DirectoryInfo(_logsFolder);
            if (!directory.Parent.Exists)
            {
                directory.Parent.Create();
            }
            if (!directory.Exists)
            {
                directory.Create();
            }

            const string format = "${date} [${level}] |>${message}";

            var fileTarget = new FileTarget("File")
            {
                Layout = format,
                FileName = Path.Combine(_logsFolder, $"{name} {DateTime.UtcNow.ToString("MM.dd hh:mm:ss.fff")}.log")
            };

            var bufferWrapper = new BufferingTargetWrapper
            {
                WrappedTarget = fileTarget,
                BufferSize = 300,
                FlushTimeout = 10,
                Name = "Buffered file",
                OptimizeBufferReuse = true
            };

            var config = new LoggingConfiguration();
            config.AddTarget(bufferWrapper);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, bufferWrapper);

            var factory = new LogFactory(config);
            //factory.ThrowConfigExceptions = true;
            //factory.ThrowExceptions = true;

            return new Logger(factory, name);
        }
    }
}