using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Elmah;
using tmg.equinox.infrastructure.logging;
using System.Web;

namespace tmg.equinox.infrastructure.exceptionhandling
{
    /// <summary>
    /// Wraps the functionality for logging of exceptions through different loggers
    /// </summary>
    public class ExceptionLoggerWrapper
    {


        #region Private Memebers

        static readonly bool enableElmahLogging = false;

        private static ILog _logger;

        private static LoggerSettings loggerSettings = new LoggerSettings();

        private class LoggerSettings
        {
            public bool EnableElmahLogging = false;
            public bool EnableTextFileLogging = false;
            public LoggerSettings() { }
        }

        #endregion Private Members

        #region Public Properties
        #endregion Public Properties

        #region Constructor

        static ExceptionLoggerWrapper()
        {
            _logger = new Logger();
            if (loggerSettings == null) loggerSettings = new LoggerSettings();
            GetAndSetLoggerSetting();
        }

        #endregion Constructor

        #region Public Methods


        public static bool LogException(Exception ex, string policyName)
        {
            bool retVal = false;

            if (loggerSettings.EnableElmahLogging)
            {
                if (HttpContext.Current != null)
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                else
                {
                    ErrorLog errorLog = ErrorLog.GetDefault(null);                    
                    errorLog.Log(new Error(ex));
                }
                
                retVal = true;
            }
            if (loggerSettings.EnableTextFileLogging)
            {
                _logger.AsyncError(ex, GetCustomAttributes(ex));
                retVal = ExceptionPolicy.HandleException(ex, policyName);
            }
            return retVal;
        }
        public static async Task<bool> LogExceptionAsync(Exception ex, string policyName)
        {
            bool retVal = false;
            retVal = await LogExceptionAsyncHandler(ex, policyName);
            return retVal;
        }
        
        #endregion Public Methods

        #region Private Methods

        private static void GetAndSetLoggerSetting()
        {
            try
            {

                if ((!String.IsNullOrEmpty(ConfigurationManager.AppSettings["EnableELMAH"]))
                    && (ConfigurationManager.AppSettings["EnableELMAH"].ToLower().Trim() == "true"))
                    loggerSettings.EnableElmahLogging = true;

                if ((!String.IsNullOrEmpty(ConfigurationManager.AppSettings["EnableLogToTextFile"]))
                    && (ConfigurationManager.AppSettings["EnableLogToTextFile"].ToLower().Trim() == "true"))
                    loggerSettings.EnableTextFileLogging = true;

                if (!(loggerSettings.EnableElmahLogging && loggerSettings.EnableTextFileLogging) && loggerSettings.EnableElmahLogging != true && loggerSettings.EnableTextFileLogging != true)
                    loggerSettings.EnableElmahLogging = true;

            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Provides the async mode for method "logexception"
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="policyName"></param>
        /// <returns></returns>
        private static async Task<bool> LogExceptionAsyncHandler(Exception ex, string policyName)
        {
            bool retVal = false;
            //If Elmah is enabled then perform the Elmah logging otherwise perform through the logger
            if (loggerSettings.EnableElmahLogging)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                retVal = true;
            }
            if (loggerSettings.EnableTextFileLogging)
            {
                _logger.AsyncError(ex, GetCustomAttributes(ex));
                retVal = ExceptionPolicy.HandleException(ex, policyName);
            }

            return retVal;
        }

        /// <summary>
        /// Returns attributes to be logged in text file with custom format.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        private static string GetCustomAttributes(Exception exception)
        {
            return string.Format("{0} & {1} & {2} & {3} & {4} & {5}",
                                 AppDomain.CurrentDomain.FriendlyName,
                                 Environment.MachineName, exception.GetType(), exception.Source, exception.Message,
                                 DateTime.Now);
        }

        #endregion Private Methods
    }
}
