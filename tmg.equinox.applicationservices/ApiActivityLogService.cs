using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.WebApi;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices
{
    public class ApiActivityLogService : IApiActivityLogService
    {
        private IUnitOfWorkAsync _unitOfWork { get; set; }

        public ApiActivityLogService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public bool Insert(ActivityLogViewModel activityLogEntry)
        {
            bool result = false;
            try
            {
                ApiActivityLog entry = new ApiActivityLog()
                {
                    Machine = activityLogEntry.Machine,
                    RequestIpAddress = activityLogEntry.RequestIpAddress,
                    RequestContentType = activityLogEntry.RequestContentType,
                    RequestContentBody = activityLogEntry.RequestContentBody,
                    RequestUri = activityLogEntry.RequestUri,
                    RequestMethod = activityLogEntry.RequestMethod,
                    RequestRouteData = activityLogEntry.RequestRouteData,
                    RequestHeaders = activityLogEntry.RequestHeaders,
                    RequestDateTime = activityLogEntry.RequestDateTime,
                    ResponseContentType = activityLogEntry.ResponseContentType,
                    ResponseContentBody = activityLogEntry.ResponseContentBody,
                    ResponseStatusCode = activityLogEntry.ResponseStatusCode,
                    ResponseHeaders = activityLogEntry.ResponseHeaders,
                    ResponseDateTime = activityLogEntry.ResponseDateTime
                };

                this._unitOfWork.RepositoryAsync<ApiActivityLog>().Insert(entry);
                this._unitOfWork.Save();

                result = true;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return result;
        }
    }
}
