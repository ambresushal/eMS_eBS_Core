using System;
using System.Web.Http;
using System.Web.Mvc;
using tmg.equinox.services.webapi.Areas.Help.Model;

namespace tmg.equinox.services.webapi.Areas.HelpPage.Controllers
{
    /// <summary>
    /// The controller that will handle requests for the help page.
    /// </summary>
    public class HelpController : Controller
    {
        public HelpController()
        {
        }

        public ActionResult Index()
        {
            ApiDescriptionGenerator generator = new ApiDescriptionGenerator();
            return View(generator.GetDecscriptor());
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult Api(string apiId)
        {
            ApiDescriptionGenerator generator = new ApiDescriptionGenerator();
            return View(generator.GetDescription(apiId));
        }
    }
}