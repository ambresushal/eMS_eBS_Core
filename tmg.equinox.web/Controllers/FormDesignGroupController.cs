using System.Web.Mvc;
using tmg.equinox.applicationservices.viewmodels.FormDesignGroup;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.applicationservices.interfaces;
using System.Collections.Generic;
using tmg.equinox.web.Framework;
using System.Threading.Tasks;

namespace tmg.equinox.web.Controllers
{
    public class FormDesignGroupController : AuthenticatedController
    {
        private IFormDesignGroupService service;        
        public FormDesignGroupController(IFormDesignGroupService service)
        {
            this.service = service;           
        }


        public ActionResult Index()
        {
            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);
            return View();
        }

        public JsonResult FormDesignGroupList(int tenantId)
        {
            IEnumerable<FormDesignGroupRowModel> formDesignGroupList = service.GetFormDesignGroupList(tenantId);
            if (formDesignGroupList == null)
            {
                formDesignGroupList = new List<FormDesignGroupRowModel>();
            }
            return Json(formDesignGroupList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FormGroupMappingList(int tenantId, int formGroupId)
        {
            IEnumerable<FormGroupFormRowModel> formDesignList = service.GetFormDesignList(tenantId, formGroupId);
            if (formDesignList == null)
            {
                formDesignList = new List<FormGroupFormRowModel>();
            }
            return Json(formDesignList, JsonRequestBehavior.AllowGet);
        }
  
        public JsonResult Add(int tenantId, string groupName)
        {
            ServiceResult result = service.AddFormGroup(User.Identity.Name, tenantId, groupName, false);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
      
        public JsonResult Update(int tenantId, int FormGroupId, string groupName)
        {
           ServiceResult result = service.UpdateFormGroup(User.Identity.Name, tenantId, FormGroupId, groupName);
           return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> UpdateFormGroupMapping(IList<FormGroupFormRowModel> formDesignRows, int tenantId, int formGroupId)
        {
            ServiceResult result = await service.UpdateFormGroupMapping(User.Identity.Name, tenantId, formGroupId, formDesignRows);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}