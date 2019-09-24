using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tmg.equinox.services.genericWebApi.Areas.Help.Model;

namespace tmg.equinox.services.genericWebApi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";           
            return View();
        }
    }
}
