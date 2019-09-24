using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using System.Diagnostics.Contracts;
using System.Transactions;
using System.Text.RegularExpressions;
using tmg.equinox.repository.extensions;
using tmg.equinox.domain.entities.Utility;
using tmg.equinox.infrastructure.logging;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.applicationservices.viewmodels.Account;

namespace tmg.equinox.applicationservices
{
    public class LoggingService : ILoggingService
    {
        #region Private Memebers
        ILog _logger;
        private IUnitOfWorkAsync _unitOfWork { get; set; }

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public LoggingService(IUnitOfWorkAsync unitOfWork, ILog log)
        {
            _logger = log;
            this._unitOfWork = unitOfWork;
        }
        #endregion Constructor

        #region Public Methods
        public void Log(object data)
        {
            _logger.AsyncInfo(data);
        }

        public void LogHeaderAndFooterCachingNotification(object data)
        {
            _logger.AsyncInfo(data);
        }


        public void LogQueryExecution(object data, Exception ex)
        {
            _logger.Info(data,ex);
        }
        public ServiceResult LogUserActivity(object data)
        {
            ServiceResult serviceResult = null;

            try
            {

                serviceResult = new ServiceResult();
                UserActivityProfile userActivityProfile = data as UserActivityProfile;
                if (userActivityProfile != null)
                {
                    string msg = userActivityProfile.Message.Length > 499 ? userActivityProfile.Message.Substring(0, 500) : userActivityProfile.Message;
                
                    Priority priority = userActivityProfile.Priority;
                    int Priority = (int)priority;
                    UserActivity userActivity = new UserActivity
                        {
                            AppDomain = userActivityProfile.AppDomain,
                            Category = userActivityProfile.Category.ToString(),
                            Event = userActivityProfile.Event.ToString(),
                            TenantID = userActivityProfile.TenantID,
                            UserName = userActivityProfile.UserName,
                            EventID = Guid.NewGuid(),
                            Host = userActivityProfile.Host,
                            Message = msg,
                            Priority = (int?)priority,
                            RequestUrl = userActivityProfile.RequestUrl,
                            Severity = userActivityProfile.Severity.ToString(),
                            TimeUtc = userActivityProfile.TimeUtc,
                            UserAgent = userActivityProfile.UserAgent
                        };

                    this._unitOfWork.RepositoryAsync<UserActivity>().Insert(userActivity);
                }
                this._unitOfWork.Save();

                serviceResult.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
                serviceResult = ex.ExceptionMessages();
                serviceResult.Result = ServiceResultStatus.Failure;
            }
            return serviceResult;
        }
        
        public async Task<ServiceResult> LogUserActivityAsync(object data)
        { 
            ServiceResult retVal= await LogUserActivityAsyncHandler(data);
            return retVal;        
        }
        
        #endregion Public Methods

        #region Private Methods
        private async Task<ServiceResult> LogUserActivityAsyncHandler(object data)
        {
            ServiceResult serviceResult = null;

            try
            {

                serviceResult = new ServiceResult();
                UserActivityProfile userActivityProfile = data as UserActivityProfile;
                if (userActivityProfile != null)
                {
                    string msg = userActivityProfile.Message.Length > 499 ? userActivityProfile.Message.Substring(0, 500) : userActivityProfile.Message;
                    Priority priority = userActivityProfile.Priority;
                    int Priority = (int)priority;
                    UserActivity userActivity = new UserActivity
                    {
                        AppDomain = userActivityProfile.AppDomain,
                        Category = userActivityProfile.Category.ToString(),
                        Event = userActivityProfile.Event.ToString(),
                        TenantID = userActivityProfile.TenantID,
                        UserName = userActivityProfile.UserName,
                        EventID = Guid.NewGuid(),
                        Host = userActivityProfile.Host,
                        Message = msg,
                        Priority = (int?)priority,
                        RequestUrl = userActivityProfile.RequestUrl,
                        Severity = userActivityProfile.Severity.ToString(),
                        TimeUtc = userActivityProfile.TimeUtc,
                        UserAgent = userActivityProfile.UserAgent
                    };

                    this._unitOfWork.RepositoryAsync<UserActivity>().Insert(userActivity);
                }
                this._unitOfWork.Save();

                serviceResult.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
                serviceResult = ex.ExceptionMessages();
                serviceResult.Result = ServiceResultStatus.Failure;
            }
            return serviceResult;
        }


        #endregion Private Methods

       
    }
}
