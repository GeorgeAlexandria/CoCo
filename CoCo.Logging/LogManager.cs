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
        public static Logger GetLogger(string name)
        {
            var appDataLocalCoco = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CoCo");
            if (!Directory.Exists(appDataLocalCoco))
            {
                Directory.CreateDirectory(appDataLocalCoco);
            }

            const string format = "${date} [${level}] |>${message}";

            FileTarget fileTarget = new FileTarget("File")
            {
                Layout = format,
                FileName = Path.Combine(appDataLocalCoco, $"{name} {DateTime.UtcNow.ToString("MM.dd hh:mm:ss.fff")}.log")
            };

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
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, bufferWrapper);

            var factory = new NLog.LogFactory(config);
            //factory.ThrowConfigExceptions = true;
            //factory.ThrowExceptions = true;

            return new Logger(factory, name);
        }
    }
}