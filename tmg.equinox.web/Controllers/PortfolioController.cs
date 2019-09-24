using System;
using System.Collections.Generic;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.Portfolio;
using tmg.equinox.web.Framework;
using System.Linq;
using tmg.equinox.identitymanagement;

namespace tmg.equinox.web.Controllers
{
    public partial class ConsumerAccountController : AuthenticatedController
    {
        // GET: /Portfolio/
        public ActionResult PortfolioSearch()
        {
            ViewBag.IsFolderLockEnable = IdentityManager.IsFolderLockEnable.ToString().ToLower();
            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);
            ViewBag.RoleId = RoleID;
            return View();
        }
        #region Private Members
        #endregion Private Members


        /// <summary>
        /// Gets the Portfolio list.
        /// </summary>
        /// <param name="tenantId"> tenant identifier.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetPortfolioList(int tenantId)
        {
            IEnumerable<PortfolioViewModel> portfolioList = this._portfolioService.GetPortfolioDetailsList(tenantId);
            if (portfolioList == null)
            {
                portfolioList = new List<PortfolioViewModel>();
            }
            ViewBag.Claims = ControllerHelper.FilterClaimsByMethod(Claims, GetActionName);
            return Json(portfolioList, JsonRequestBehavior.AllowGet);
        }

    }
}