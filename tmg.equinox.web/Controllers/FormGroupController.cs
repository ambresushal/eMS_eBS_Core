using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.web.Framework;

namespace tmg.equinox.web.Controllers
{
    public class FormGroupController : AuthenticatedController
    {
        #region Private Members
        #endregion Private Members

        #region Constructor
        public FormGroupController()
        {
        }
        #endregion Constructor

        public ActionResult Index()
        {
            return View();
        }

    }
}