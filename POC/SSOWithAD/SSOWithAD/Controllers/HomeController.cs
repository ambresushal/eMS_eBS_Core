using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IdentityManagement;

namespace SSOWithAD.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string userRoleName = string.Empty;
            userRoleName = IdentityManager.GetRole(HttpContext.User.Identity.Name);
            
            if (!string.IsNullOrEmpty(userRoleName))
                ViewBag.UserRole = userRoleName;
            else
                ViewBag.UserRole = null;

            return View();
        }
    }
}