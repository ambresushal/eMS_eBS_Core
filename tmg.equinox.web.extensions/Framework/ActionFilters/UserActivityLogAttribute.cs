using System;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.Account;
using tmg.equinox.dependencyresolution;
using tmg.equinox.identitymanagement.Extensions;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.identitymanagement;
using System.Security.Claims;
using tmg.equinox.caching.client;
using System.Configuration;

namespace tmg.equinox.web.Framework.ActionFilters
{
    public class UserActivityLogAttribute : ActionFilterAttribute, IActionFilter
    {
        #region Private Memebers
        private ILoggingService _loggingService { get; set; }
        #endregion Private Members

        #region Public Properties

        public int Category { get; set; }

        public int Priority { get; set; }

        public int Severity { get; set; }

        public int ActivityEvent { get; set; }

        public string Message { get; set; }

        #endregion Public Properties

        #region Constructor
        #endregion Constructor

        #region Public Methods

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var actionDescriptor = filterContext.ActionDescriptor;
            string controllerName = actionDescriptor.ControllerDescriptor.ControllerName;
            string actionName = actionDescriptor.ActionName;
            var userName = (request.IsAuthenticated) ? filterContext.HttpContext.User.Identity.Name : "Anonymous";

            string routeId = string.Empty;
            //string requestUrl=string.Empty;

            //if(string.IsNullOrEmpty(request.RawUrl.ToString())
            //    .Equals(string.Empty)) requestUrl=string.Format("/{0}/{1}", actionName, controllerName);

            //string requestUrl = string.IsNullOrEmpty(request.RawUrl) ? string.Format("/{0}/{1}", actionName, controllerName) : request.RawUrl;

            if (filterContext.RouteData.Values["id"] != null)
            {
                routeId = filterContext.RouteData.Values["id"].ToString();
            }

            UserActivityProfile userActivityProfile = new UserActivityProfile();

            AppDomain root = AppDomain.CurrentDomain;
            var ipAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress;
            int tenantID = 0;
            try
            {
                var identity = (filterContext.HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity);
                var claimPrinciple = (ClaimsPrincipal)filterContext.HttpContext.User;
                foreach (var claimIdentity in claimPrinciple.Identities)
                {
                    if (!(claimIdentity is System.Security.Principal.WindowsIdentity))
                    {
                        if (claimIdentity is ClaimsIdentity)
                        {
                            identity = claimIdentity;
                        }
                    }
                }
                if (identity.IsAuthenticated)
                    tenantID = (identity as ClaimsIdentity).GetTenantId();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;

            }

            userActivityProfile.Host = System.Environment.MachineName;

            userActivityProfile.Category = (Category)Category;
            userActivityProfile.TimeUtc = DateTime.Now;
            userActivityProfile.Host = System.Environment.MachineName;
            userActivityProfile.TenantID = tenantID;
            userActivityProfile.Event = (Event)ActivityEvent;
            userActivityProfile.UserName = userName;
            userActivityProfile.RequestUrl = request.Url.AbsoluteUri.ToString();
            userActivityProfile.AppDomain = root.FriendlyName;
            userActivityProfile.UserAgent = request.Browser.Type;
            userActivityProfile.Priority = (Priority)Priority;
            userActivityProfile.Severity = (Severity)Severity;
            userActivityProfile.Message = Message;

            _loggingService = UnityConfig.container.Resolve<ILoggingService>();

            _loggingService.LogUserActivityAsync(userActivityProfile);

            if ((!String.IsNullOrEmpty(ConfigurationManager.AppSettings["EnableCachingLogToAuditFile"])) && (ConfigurationManager.AppSettings["EnableCachingLogToAuditFile"].ToLower().Trim() == "true"))
                _loggingService.LogHeaderAndFooterCachingNotification(userActivityProfile.Message);

            base.OnActionExecuted(filterContext);
        }
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods
    }
}
