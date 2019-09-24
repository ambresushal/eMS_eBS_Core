using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
//using tmg.equinox.caching.Interfaces;
using tmg.equinox.identitymanagement;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.identitymanagement.Extensions;
using tmg.equinox.web.Controllers;
using tmg.equinox.web.Framework.ActionFilters;
using tmg.equinox.caching.client;
using System.Collections.Specialized;
using System.Configuration;
//using tmg.equinox.caching.Stores;
using System.Threading.Tasks;
using tmg.equinox.caching;
using System.Linq;
using tmg.equinox.reporting.Base.Model;
using tmg.equinox.reporting.Base;
using System.IO;
using tmg.equinox.domain.entities.Enums;

namespace tmg.equinox.web.Framework
{
    //[Authorize]

    [PostAuthorize]
    public class BaseController : Controller
    {
        public BaseController()
        {
            GetApplicationName();
            GetClientName();
        }
        /// <summary>
        /// Retrieves the CurrentUserId as stored in the claims principal
        /// </summary>
        /// <remarks>
        /// Using this method requires user is authenticated and authorized
        /// </remarks>
        protected int? CurrentUserId
        {
            get
            {
                int? userId = null;
                int tenantId = 1;

                if (CurrentUserClaimsIdentity != null)
                {
                    int i = 0;
                    if (int.TryParse(CurrentUserClaimsIdentity.GetUserId(), out i))
                        userId = i;
                    else if (!string.IsNullOrEmpty(CurrentUserName) && RoleID > 0)
                    {
                        userId = IdentityManager.GetUserId(tenantId, CurrentUserName, RoleID);
                        CurrentUserClaimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));
                    }

                }
                return userId;
            }
        }

        protected string CurrentUserName
        {
            get
            {
                string userName = string.Empty;
                List<Claim> nameIdentifierClaims = new List<Claim>();

                if (CurrentUserClaimsIdentity != null)
                {
                    userName = CurrentUserClaimsIdentity.GetUserName();
                    userName = userName.Substring(userName.IndexOf(@"\") + 1);

                    if (string.IsNullOrEmpty(userName))

                        nameIdentifierClaims = CurrentUserClaimsIdentity.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).ToList<Claim>();
                    if (nameIdentifierClaims != null && nameIdentifierClaims.Count > 0)
                    {
                        userName = nameIdentifierClaims.LastOrDefault().Value;
                    }
                }
                return userName;
            }
        }

        protected ClaimsIdentity CurrentUserClaimsIdentity
        {
            get
            {
                //for windows we need to get from other identities
                ClaimsIdentity claimIdentity = GetClaimIdentity();
                if (claimIdentity == null)
                    return HttpContext.User.Identity as ClaimsIdentity;
                else
                    return claimIdentity;
            }
        }
        public void GetApplicationName()
        {
            ViewBag.ApplicationName = tmg.equinox.config.Config.GetApplicationName();
        }

        public void GetClientName()
        {
            var appNameSetting = ConfigurationManager.AppSettings["clientName"];
            string appName = "floridablue";
            if (appNameSetting != null)
            {
                appName = appNameSetting.ToString() ?? appName;
            }
            ViewBag.ClientName = appName;
        }

        private ClaimsIdentity GetClaimIdentity()
        {
            var claimPrinciple = (ClaimsPrincipal)HttpContext.User;
            foreach (var claimIdentity in claimPrinciple.Identities)
            {
                if (!(claimIdentity is System.Security.Principal.WindowsIdentity))
                {
                    if (claimIdentity is ClaimsIdentity)
                    {
                        return claimIdentity;
                    }
                }
            }
            return null;
        }

        protected int TenantID
        {
            get
            {
                return CurrentUserClaimsIdentity.GetTenantId();
            }
        }

        /// <summary>
        /// Retrieves current user's RoleId
        /// </summary>
        protected int RoleID
        {
            get
            {
                return CurrentUserClaimsIdentity.GetRoleId();
            }
        }

        protected bool IsSuperUser()
        {
            var roleId = RoleID;

            if (roleId == (int)Role.TMGSuperUser)
            {
                return true;
            }
            if (roleId == (int)Role.WCSuperUser)
            {
                return true;
            }
            return false;
        }
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            try
            {
                base.Initialize(requestContext);
                //set Http headers
                if (requestContext.HttpContext.Session != null && !requestContext.HttpContext.Session.IsNewSession)
                {
                  //  requestContext.HttpContext.Response.AppendHeader("Refresh", Convert.ToString((requestContext.HttpContext.Session.Timeout * 60) - 3));
                    requestContext.HttpContext.Response.AddHeader("Cache-Control", "no-cache");
                    requestContext.HttpContext.Response.Expires = 0;
                    requestContext.HttpContext.Response.Cache.SetMaxAge(new TimeSpan(0, 0, 0, 0));
                    requestContext.HttpContext.Response.AddHeader("Pragma", "no-cache");
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            //clear content since only error has to be displayed
            filterContext.HttpContext.Response.ClearContent();
            string currentControllerName = (string)filterContext.RouteData.Values["controller"];
            string currentActionName = (string)filterContext.RouteData.Values["action"];
            string _ExceptionSource = " Controller: " + currentControllerName + " " + " Action: " + currentActionName;

            filterContext.ExceptionHandled = true;

            /*var model = new HandleErrorInfo(filterContext.Exception, currentControllerName, currentActionName);
            ViewDataDictionary<HandleErrorInfo> viewDataDictionary = new ViewDataDictionary<HandleErrorInfo>(model);
            var result = new ViewResult
            {
                ViewName = "~/Views/Error/Index.cshtml",

                ViewData = viewDataDictionary,
                
            };
            filterContext.Result = result;*/

            ExceptionPolicyWrapper.HandleExceptionAsync(filterContext.Exception, ExceptionPolicies.ExceptionShielding);
            var controller = new ErrorController();
            controller.ViewData.Model = new HandleErrorInfo(filterContext.Exception, currentControllerName, currentActionName);

            var routeData = new RouteData();
            var action = "Index";
            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = action;

            filterContext.RequestContext = new RequestContext(filterContext.HttpContext, routeData);
            ((IController)controller).Execute(filterContext.RequestContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsLocal)
            {
                if (filterContext.HttpContext.User.Identity.IsAuthenticated == false)
                    filterContext.HttpContext.Response.StatusCode = 999;
            }
        }

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding
            };
        }

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }
        #region Public properties

        public string GetRequestUrl
        {
            get
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                return string.Format("/{0}/{1}", actionName, controllerName);
            }
        }

        public string GetControllerName
        {
            get { return ControllerContext.RouteData.Values["controller"].ToString(); }
        }

        public string GetActionName
        {
            get { return ControllerContext.RouteData.Values["action"].ToString(); }
        }

        public List<Claim> Claims
        {
            get
            {
                List<Claim> claims = null;

                CachingManager.GetCachedClaims(CurrentUserId.ToString(), out claims);

                if (claims == null)
                {
                    //ClaimsIdentity claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
                    //claims = ((List<Claim>)claimsIdentity.Claims);

                    claims = IdentityManager.GetClaimsFromIdentity(CurrentUserClaimsIdentity as ClaimsIdentity);
                }

                return IdentityManager.GetIdentityClaims(claims, GetControllerName, GetActionName);

            }
        }

        public bool CachingEnabled
        {
            get
            {
                return CacheConfig.EnableCaching;
            }

        }
        #endregion
    }
}