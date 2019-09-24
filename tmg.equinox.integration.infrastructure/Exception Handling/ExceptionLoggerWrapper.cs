using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.Common;
using tmg.equinox.integration.infrastructure.logging;

namespace tmg.equinox.integration.infrastructure.exceptionhandling
{
    /// <summary>
    /// Wraps the functionality for logging of exceptions through different loggers
    /// </summary>
    public class ExceptionLoggerWrapper
    {
        #region Private Memebers       
        private static ILog _logger;            
        #endregion Private Members
       
        #region Constructor

        static ExceptionLoggerWrapper()
        {
            _logger = new Logger();          
        }
        #endregion Constructor

        #region Public Methods
        public static bool LogException(Exception ex, string policyName)
        {
            bool retVal = false;
            _logger.Error(ex.Message +"\n" +ex.StackTrace);
            retVal = ExceptionPolicy.HandleException(ex, policyName);            
            return retVal;
        }       
        #endregion Public Methods               
    }
}
