using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.applicationservices.viewmodels.PrintTemplate;
using tmg.equinox.applicationservices.viewmodels.Reporting;
using tmg.equinox.web.FormInstance;
using tmg.equinox.web.Framework;

namespace tmg.equinox.web.Controllers
{
    public class PrintTemplateController : UnAuthenticatedController
    {
        #region Private Variables
        private IFormDesignService _formDesignService;
        private ITemplateReportService _templateReportService;
        #endregion
        #region Constructor

        public PrintTemplateController(IFormDesignService service, ITemplateReportService templateReportService)
        {
            this._formDesignService = service;
            this._templateReportService = templateReportService;
        }

        #endregion
        #region ActionResult
        //[NonAction]
        public ActionResult PDFConfiguration()
        {
            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);
            ViewBag.RoleId = RoleID;    
            return View("Index");
        }
        public JsonResult FormDesignList(int tenantId)
        {
            IEnumerable<FormDesignRowModel> formDesignList = null;
            formDesignList = _formDesignService.GetFormDesignList(tenantId);

            if (formDesignList == null)
            {
                formDesignList = new List<FormDesignRowModel>();
            }
            return Json(formDesignList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FormDesignVersionList(int tenantId, int formDesignId)
        {
            IEnumerable<FormDesignVersionRowModel> formDesignVersionList = _formDesignService.GetFormDesignVersionList(tenantId, formDesignId);
            if (formDesignVersionList == null)
            {
                formDesignVersionList = new List<FormDesignVersionRowModel>();
            }
            return Json(formDesignVersionList, JsonRequestBehavior.AllowGet);
        }

        //Get Document Design Template List
        public JsonResult DocumentTemplateList(int tenantId)
        {
            IEnumerable<TemplateViewModel> documentTemplateList = null;
            documentTemplateList = _templateReportService.GetDocumentTemplateList(tenantId);

            if (documentTemplateList == null)
            {
                documentTemplateList = new List<TemplateViewModel>();
            }
            return Json(documentTemplateList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]        
        public ActionResult Add(int tenantId, int formDesignId, int formDesignVersionId, string templateName, string description)
        {
            ServiceResult result = _templateReportService.AddDocumentTemplate(tenantId, formDesignId, formDesignVersionId, templateName,description, User.Identity.Name);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method Returns the List of UI Elements for Form Design Version
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <returns>List of UI Elements for Form Design Version</returns>
        public JsonResult LoadTemplateDesignUIElementList(int tenantId, int formDesignVersionId, int templateId)
        {
            IEnumerable<TemplateViewModel> uiElementList = _templateReportService.LoadTemplateDesignUIElement(tenantId, formDesignVersionId, templateId);
            if (uiElementList == null)
            {
                uiElementList = new List<TemplateViewModel>();
            }
            return Json(uiElementList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SetTemplateDesignUIElement(int tenantId, string uiElementList, int templateID)
        {
           
            //get json object from json string
           List<TemplateViewModel> templateUIElementList = JsonConvert.DeserializeObject<List<TemplateViewModel>>(uiElementList);
            ServiceResult result = _templateReportService.UpdateTemplateDesignUIElement(tenantId, templateUIElementList, templateID, User.Identity.Name);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult deleteDesignTemplate(int tenantId, int templateId)
        {
            ServiceResult result = _templateReportService.DeleteDocumentTemplate(tenantId, templateId);

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        #endregion 
    }
}