using System;
using NLog;

namespace CoCoLog
{
    public sealed class Logger : IDisposable
    {
        /// NOTE: Handle <see cref="NLog.LogFactory"/> to dispose inner configurations and targets
        private LogFactory _factory;

        private NLog.Logger _logger;

        internal Logger(LogFactory factory, string loggerName)
        {
            _factory = factory;
            _logger = _factory.GetLogger(loggerName);
        }

        // NOTE: it's tiny optimization for string building

        public void Debug(string message) => _logger.Debug(message);

        public void Debug<TArg>(string message, TArg arg) => _logger.Debug(message, arg);

        public void Debug<TArg1, TArg2>(string message, TArg1 arg1, TArg2 arg2) => _logger.Debug(message, arg1, arg2);

        public void Debug<TArg1, TArg2, TArg3>(string message, TArg1 arg1, TArg2 arg2, TArg3 arg3) => _logger.Debug(message, arg1, arg2, arg3);

        public void Info(string message) => _logger.Info(message);

        public void Info<TArg>(string message, TArg arg) => _logger.Info(message, arg);

        public void Info<TArg1, TArg2>(string message, TArg1 arg1, TArg2 arg2) => _logger.Info(message, arg1, arg2);

        public void Info<TArg1, TArg2, TArg3>(string message, TArg1 arg1, TArg2 arg2, TArg3 arg3) => _logger.Info(message, arg1, arg2, arg3);

        public void Warn(string message) => _logger.Warn(message);

        public void Warn<TArg>(string message, TArg arg) => _logger.Warn(message, arg);

        public void Warn<TArg1, TArg2>(string message, TArg1 arg1, TArg2 arg2) => _logger.Warn(message, arg1, arg2);

        public void Warn<TArg1, TArg2, TArg3>(string message, TArg1 arg1, TArg2 arg2, TArg3 arg3) => _logger.Warn(message, arg1, arg2, arg3);

        public void Error(string message) => _logger.Error(message);

        public void Error<TArg>(string message, TArg arg) => _logger.Error(message, arg);

        public void Error<TArg1, TArg2>(string message, TArg1 arg1, TArg2 arg2) => _logger.Error(message, arg1, arg2);

        public void Error<TArg1, TArg2, TArg3>(string message, TArg1 arg1, TArg2 arg2, TArg3 arg3) => _logger.Error(message, arg1, arg2, arg3);

        void IDisposable.Dispose()
        {
            _logger = null;
            try
            {
                _factory.Dispose();
            }
            finally
            {
                _factory = null;
                // NOTE: Just optomization
                GC.SuppressFinalize(this);
            }
        }
    }
}