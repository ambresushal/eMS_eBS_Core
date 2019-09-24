using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FormDesign;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface ILoggingService
    {
        //Logging Interfaces
        void Log(object data);
        void LogHeaderAndFooterCachingNotification(object data);
        /// <summary>
        /// Provides mechanism to log the user activity in the event sink.
        /// </summary>
        /// <param name="data"></param>
        ServiceResult LogUserActivity(object data);
        /// <summary>
        /// Provides asynchronous operation
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<ServiceResult> LogUserActivityAsync(object data);

        void LogQueryExecution(Object data,Exception ex);
    }
}
