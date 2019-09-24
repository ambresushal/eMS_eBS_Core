using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.Consortium;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.web.Framework;

namespace tmg.equinox.web.Controllers
{
    public class ConsortiumController : AuthenticatedController
    {
        private IConsortiumService _consortiumService { get; set; }

        public ConsortiumController()
        {
        }

        public ConsortiumController(IConsortiumService consortiumService)
        {
            this._consortiumService = consortiumService;
        }

        public ActionResult Index()
        {
            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);
            return View();
        }

        [HttpGet]
        public JsonResult GetConsortiumList(int tenantID, GridPagingRequest gridPagingRequest)
        {

            GridPagingResponse<ConsortiumViewModel> consortiumList = this._consortiumService.GetConsortiumList(tenantID, gridPagingRequest);
               
            ViewBag.Claims = ControllerHelper.FilterClaimsByMethod(Claims, GetActionName);
            return Json(consortiumList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddConsortium(string consortiumName, int tenantID)
        {
            ServiceResult result = this._consortiumService.AddConsortium(consortiumName, tenantID, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateConsortium(int consortiumID, string consortiumName, int tenantID)
        {
            ServiceResult result = this._consortiumService.UpdateConsortium(consortiumID, consortiumName, tenantID, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetConsortiumForDropdown(int tenantID)
        {
            IEnumerable<ConsortiumViewModel> consortiumList = this._consortiumService.GetConsortiumForDropdown(tenantID);
            if (consortiumList == null)
            {
                consortiumList = new List<ConsortiumViewModel>();
            }
            return Json(consortiumList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetConsortium(int folderVersionID)
        {
            ConsortiumViewModel consortium = this._consortiumService.GetConsortium(folderVersionID);
           
            return Json(consortium, JsonRequestBehavior.AllowGet);
        }
    }
}