using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.web.Framework;
namespace tmg.equinox.web.Controllers
{

    public class ErrorController : Controller
    {
        public ActionResult PageNotFound()
        {
            return View();
        }
        public ActionResult ServerError()
        {
            return View();
        }

        public ActionResult LoginFailPage()
        {
            return View();
        }

        public ActionResult RoleNotFoundPage()
        {
            return View();
        }

        public ActionResult Index()
        {
            var userMailId = WebConfigurationManager.AppSettings["ErrorPageMailID"];
            var enableShowError = WebConfigurationManager.AppSettings["EnableShowError"];
            var currentURL = HttpContext.Request.Url;
            ViewBag.enableShowError = enableShowError;
            ViewBag.currentURL = currentURL;
            ViewData["UserMailID"] = userMailId;
            return View();
        }
        public ActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        public void LogJavaScriptError(string errorMessage)
        {
            string[] message = errorMessage.Split('$');

            string sourceOld = message[0];
            string[] sourceArray = sourceOld.Split('/');
            string source = sourceArray.Last();

            string exceptionMessage = message[1];

            string statuscode = string.Empty;

            ExceptionPolicyWrapper.HandleException(new JavaScriptException(exceptionMessage, source, statuscode), ExceptionPolicies.ExceptionShielding);

        }
    }
}