using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.documentcomparer.RepeaterCompareUtils;
using tmg.equinox.web.FormDesignManager;
using tmg.equinox.web.FormInstanceProcessor.SourceTargetDataManager.SourceHandler;
using tmg.equinox.web.PBPView;

namespace tmg.equinox.web.Controllers
{
    public class PBPViewActionController : AuthenticatedController
    {
        private IFormInstanceViewImpactLogService _impactLogService;
        private IUIElementService _uiElementService;
        private IFormInstanceService _formInstanceService;
        private IFormDesignService _formDesignServices;
        public PBPViewActionController(IFormInstanceViewImpactLogService impactLogService, IUIElementService uiElementService, IFormInstanceService formInstanceService, IFormDesignService formDesignServices)
        {
            this._impactLogService = impactLogService;
            this._uiElementService = uiElementService;
            this._formInstanceService = formInstanceService;
            this._formDesignServices = formDesignServices;
        }

        [HttpPost]
        public JsonResult SaveDocumentViewImpactLog(int tenantId, int formInstanceId, int folderId, int folderVersionId, int formDesignId, int formDesignVersionId, string activityLogFormInstanceData)
        {
            ServiceResult result = null;
            List<SourceElement> impactList = new List<SourceElement>();
            List<ActivityLogModel> activityLog = JsonConvert.DeserializeObject<List<ActivityLogModel>>(activityLogFormInstanceData);
            // Get Last Updated Distinct field as we need to show impacted field for each field so no need to show duplicate field as impacted field will be same and we need take last change for selected field.   
            activityLog = activityLog.OrderByDescending(y => y.UpdatedLast).Distinct(new ActivityLogEqualityComparer()).ToList();
            // JSON.stringify looses datetime format, need to convert to localTime before saving impacted fields. 
            activityLog.ForEach(x => x.UpdatedLast = x.UpdatedLast.ToLocalTime());
            var design = FormDesignGroupMapManager.Get("PBPView", _formDesignServices);
            var formInstance = _formInstanceService.GetViewByAnchor(folderVersionId, design.FormDesignID, formInstanceId);
            if (formInstance != null)
            {
                ViewImpactCacheManager cache = new ViewImpactCacheManager();
                var compiledList = cache.Get(formInstance.FormDesignVersionId, _uiElementService);
                // Get PBPView ActivityLog that was added by expression rule on document save.
                List<ActivityLogModel> pbpViewActivityLog = _impactLogService.GetFormInstanceActivityLogData(formInstance.FormInstanceId, compiledList);
                DocumentViewImpactManager impactManager = new DocumentViewImpactManager();
                impactList = impactManager.GetImpactedFields(activityLog, compiledList, pbpViewActivityLog, formInstance.FormInstanceId, formInstance.FormDesignVersionId, design.FormDesignID, _impactLogService);
                result = _impactLogService.SaveFormInstanceImpactlogData(formInstanceId, folderId, folderVersionId, formDesignId, formDesignVersionId, impactList);
            }
            return Json(impactList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetPBPViewImpactData(int formInstanceId)
        {
            List<SourceElement> activityLogDataList = this._impactLogService.GetFormInstanceImpactLogData(formInstanceId);

            return Json(activityLogDataList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SetTempDataForNavigation(int anchorFormInstanceId, int folderVersionId, string sectionName, string sectionGeneratedName)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                var design = FormDesignGroupMapManager.Get("PBPView", _formDesignServices);
                var formInstance = _formInstanceService.GetViewByAnchor(folderVersionId, design.FormDesignID, anchorFormInstanceId);
                if (formInstance != null)
                {
                    TempData["ViewFormInstanceID"] = formInstance.FormInstanceId;
                    TempData["SectionName"] = sectionName;
                    TempData["SectionGeneratedName"] = sectionGeneratedName;
                }
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ExportImpactLogData(int formInstanceID, string formInstanceName)
        {
            List<SourceElement> impactJsonObject = _impactLogService.GetFormInstanceImpactLogData(formInstanceID);
            DocumentViewImpactManager viewImpactManager = new DocumentViewImpactManager();
            byte[] reportBytes = viewImpactManager.GeneratePBPViewImpactExcelReport(impactJsonObject, formInstanceName);
            string fileName = "PBP View Impact Report";
            var fileDownloadName = fileName + ".xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(reportBytes, contentType, fileDownloadName);
        }

    }
}