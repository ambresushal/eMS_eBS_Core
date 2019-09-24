using Hangfire.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using tmg.equinox.infrastructure.exceptionhandling;

namespace tmg.equinox.hangfire.Logger
{
    public class CustomHangfireLogger : ILog
    {
        private static readonly tmg.equinox.core.logging.Logging.ILog _logger =  tmg.equinox.core.logging.Logging.LogProvider.GetCurrentClassLogger();
        public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null)
        {
            if (messageFunc == null)
            {
                return true;
            }

            var text = messageFunc();
            tmg.equinox.core.logging.Logging.LogLevel nlogLevel = core.logging.Logging.LogLevel.Info;

            switch (logLevel)
            {
                case LogLevel.Trace:
                    nlogLevel = core.logging.Logging.LogLevel.Trace;
                    break;
                case LogLevel.Debug:
                    nlogLevel = core.logging.Logging.LogLevel.Debug;
                    break;
                case LogLevel.Info:
                    nlogLevel = core.logging.Logging.LogLevel.Info;
                    break;
                case LogLevel.Warn:
                    nlogLevel = core.logging.Logging.LogLevel.Warn;
                    break;
                case LogLevel.Error:
                    nlogLevel = core.logging.Logging.LogLevel.Error;
                    // ExceptionLoggerWrapper.LogException(exception, "Hangfire");
                    break;
                case LogLevel.Fatal:
                    nlogLevel = core.logging.Logging.LogLevel.Fatal;
                    break;
                default:
                    break;
            }
            _logger.Log(nlogLevel, messageFunc, exception);
            return true;
        }

        //public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null)
        //{
        //    if (messageFunc == null)
        //    {
        //        return true;
        //    }

        //    var text = messageFunc();

        //    switch (logLevel)
        //    {
        //        case LogLevel.Trace:
        //            Console.WriteLine($"Trace: {text}");
        //            break;
        //        case LogLevel.Debug:
        //            Console.WriteLine($"Debug: {text}");
        //            break;
        //        case LogLevel.Info:
        //            Console.WriteLine($"Info: {text}");
        //            break;
        //        case LogLevel.Warn:
        //            Console.WriteLine($"Warn: {text}");
        //            break;
        //        case LogLevel.Error:
        //            Console.WriteLine($"Error: {text} ex: {exception.Message}");
        //            break;
        //        case LogLevel.Fatal:
        //            Console.WriteLine($"Fatal: {text}");
        //            break;
        //        default:
        //            break;
        //    }

        //    return true;
        //}

        public bool SetLogWriter()
        {
            return true;
        }


    }
}
