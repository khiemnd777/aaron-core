using System;
using Aaron.Core.Domain.Accounts;
using Aaron.Core.Domain.Logging;

namespace Aaron.Core.Services.Logging
{
    public static class LoggingExtensions
    {
        public static void Debug(this ILogger logger, string message, Exception exception = null, Account account = null)
        {
            FilteredLog(logger, LogLevel.Debug, message, exception, account);
        }
        public static void Information(this ILogger logger, string message, Exception exception = null, Account account = null)
        {
            FilteredLog(logger, LogLevel.Information, message, exception, account);
        }
        public static void Warning(this ILogger logger, string message, Exception exception = null, Account account = null)
        {
            FilteredLog(logger, LogLevel.Warning, message, exception, account);
        }
        public static void Error(this ILogger logger, string message, Exception exception = null, Account account = null)
        {
            FilteredLog(logger, LogLevel.Error, message, exception, account);
        }
        public static void Fatal(this ILogger logger, string message, Exception exception = null, Account account = null)
        {
            FilteredLog(logger, LogLevel.Fatal, message, exception, account);
        }

        private static void FilteredLog(ILogger logger, LogLevel level, string message, Exception exception = null, Account account = null)
        {
            //don't log thread abort exception
            if ((exception != null) && (exception is System.Threading.ThreadAbortException))
                return;

            if (logger.IsEnabled(level))
            {
                string fullMessage = exception == null ? string.Empty : exception.ToString();
                logger.InsertLog(level, message, fullMessage, account);
            }
        }
    }
}
