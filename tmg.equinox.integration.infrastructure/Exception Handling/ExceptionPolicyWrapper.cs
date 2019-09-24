using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.Common;
using tmg.equinox.integration.infrastructure.logging;



namespace tmg.equinox.integration.infrastructure.exceptionhandling
{
    /// <summary>
    /// This class provides wrapper for exception policies, exception logging.
    /// </summary>
    public class ExceptionPolicyWrapper
    {
        #region Private Memebers
        private static ExceptionManager _exManager;
        #endregion Private Members

        #region Public Properties
        #endregion Public Properties

        #region Constructor
        static ExceptionPolicyWrapper()
        {
            ExceptionPolicyFactory policyFactory = new ExceptionPolicyFactory();
            _exManager = policyFactory.CreateManager();
            ExceptionPolicy.SetExceptionManager(_exManager);
        } 
        #endregion Constructor

        #region Public Methods

        public static bool HandleException(Exception ex, string policyName)
        {
            return ExceptionLoggerWrapper.LogException(ex, policyName);
        }

       
        public static bool HandleExceptionAsync(Exception ex, string policyName)
        {            
            bool result = ExceptionLoggerWrapper.LogException(ex, policyName);
            return result;
        }
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods
    }
}
