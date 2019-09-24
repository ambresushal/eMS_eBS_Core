using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Security.Claims;
using System.Web.Configuration;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Microsoft.AspNet.Identity;
using tmg.equinox.applicationservices.viewmodels.Account;
using tmg.equinox.web.Framework;
using tmg.equinox.web.Framework.ActionFilters;

namespace tmg.equinox.web.Controllers
{
    public class MenuController : AuthenticatedController
    {
        //
        // GET: /Menu/
        public ActionResult Index()
        {
            return View();
        }
       
        //[DonutOutputCache(CacheProfile = "DonutCachingHeader")]
        //[UserActivityLog(Priority = (int)Priority.Medium, Category = (int)Category.Audit, Message = "Caching Header Menus", Severity = (int)Severity.Information, ActivityEvent = (int)Event.Caching)]
        [ChildActionOnly]
        //[HttpGet]
        public ActionResult Menus()
        {
            var authenticationSectionCollection = (NameValueCollection)ConfigurationManager.GetSection("authenticationSection");
            ViewBag.UseAuthentication = Convert.ToBoolean(authenticationSectionCollection["UseAuthentication"]);
            ViewBag.DummyAuthenticationEnabled = Convert.ToBoolean(authenticationSectionCollection["DummyAuthenticationEnabled"]);

           
            if (base.CurrentUserId == null)
            {
                RedirectToAction("LogOn", "Account");
            }
            else 
            {
                ViewBag.UserIdentity = User.Identity as ClaimsIdentity;
                return View();
            }

            return null;
        }

       
        //[DonutOutputCache(CacheProfile = "DonutCachingFooter")]
       // [UserActivityLog(Priority = (int)Priority.Medium, Category = (int)Category.Audit, Message = "Caching Footer", Severity = (int)Severity.Information, ActivityEvent = (int)Event.Caching)]
        [ChildActionOnly]
        //[HttpGet]
        public ActionResult Footer()
        {
            return View();
        }
    }
}






