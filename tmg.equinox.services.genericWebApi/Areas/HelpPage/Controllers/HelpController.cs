using System;
using System.Web.Http;
using System.Web.Mvc;
using tmg.equinox.services.genericWebApi.Areas.HelpPage.ModelDescriptions;
using tmg.equinox.services.genericWebApi.Areas.HelpPage.Models;
using tmg.equinox.services.genericWebApi.Areas.Help.Model;


namespace tmg.equinox.services.genericWebApi.Areas.HelpPage.Controllers
{
    /// <summary>
    /// The controller that will handle requests for the help page.
    /// </summary>
    public class HelpController : Controller
    {
        private const string ErrorViewName = "Error";

        //public HelpController()
        //    : this(GlobalConfiguration.Configuration)
        //{
        //}

        //public HelpController(HttpConfiguration config)
        //{
        //    Configuration = config;
        //}
        protected static HttpConfiguration Configuration
        {
            get { return GlobalConfiguration.Configuration; }
        }


      //  public HttpConfiguration Configuration { get; private set; }

        public ActionResult Index()
        {
            ViewBag.DocumentationProvider = Configuration.Services.GetDocumentationProvider();
            return View(Configuration.Services.GetApiExplorer().ApiDescriptions);
            //ApiDescriptionGenerator generator = new ApiDescriptionGenerator();
            //return View(generator.GetDecscriptor());
        }

          [System.Web.Mvc.HttpGet]
        public ActionResult Api(string model)
        {
            string ApiId = Configuration.Services.GetApiExplorer().ApiDescriptions[0].ID;
            if (!String.IsNullOrEmpty(model))
            {
                HelpPageApiModel apiModel = Configuration.GetHelpPageApiModel(model);
                if (apiModel != null)
                {
                    return View(apiModel);
                }
            }

            return View(ErrorViewName);
        }

        public ActionResult ResourceModel(string modelName)
        {
            if (!String.IsNullOrEmpty(modelName))
            {
                ModelDescriptionGenerator modelDescriptionGenerator = Configuration.GetModelDescriptionGenerator();
                ModelDescription modelDescription;
                if (modelDescriptionGenerator.GeneratedModels.TryGetValue(modelName, out modelDescription))
                {
                    return View(modelDescription);
                }
            }

            return View(ErrorViewName);
        }
    }
}