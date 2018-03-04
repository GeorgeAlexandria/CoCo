using System.Diagnostics;

namespace CoCo
{
    /// <summary>
    /// <see cref="Log"/> is used only in the debug configuration, <see cref="ConditionalAttribute"/>
    /// and Preprocessor Directives are used for it.
    /// </summary>
    internal static class Log
    {
#if DEBUG
        private static CoCoLog.Logger _logger;
#endif

        private const string _debug = "DEBUG";

        static Log()
        {
#if DEBUG
            _logger = CoCoLog.LogManager.GetLogger("Classifier");
#endif
        }

        // NOTE: it's tiny optimization for string building
        [Conditional(_debug)]
        internal static void Debug<TArg>(string message, TArg arg)
        {
#if DEBUG
            _logger.Debug(message, arg);
#endif
        }

        [Conditional(_debug)]
        internal static void Debug<TArg1, TArg2>(string message, TArg1 arg1, TArg2 arg2)
        {
#if DEBUG
            _logger.Debug(message, arg1, arg2);
#endif
        }

        [Conditional(_debug)]
        internal static void Debug<TArg1, TArg2, TArg3>(string message, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
#if DEBUG
            _logger.Debug(message, arg1, arg2, arg3);
#endif
        }

        [Conditional(_debug)]
        internal static void Info<TArg>(string message, TArg arg)
        {
#if DEBUG
            _logger.Info(message, arg);
#endif
        }

        [Conditional(_debug)]
        internal static void Info<TArg1, TArg2>(string message, TArg1 arg1, TArg2 arg2)
        {
#if DEBUG
            _logger.Info(message, arg1, arg2);
#endif
        }

        [Conditional(_debug)]
        internal static void Info<TArg1, TArg2, TArg3>(string message, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
#if DEBUG
            _logger.Info(message, arg1, arg2, arg3);
#endif
        }
    }
}