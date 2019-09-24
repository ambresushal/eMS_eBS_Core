using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.web.FormInstance;

namespace tmg.equinox.web.Controllers
{
    public class FormInstanceRuleExecutionLogController : AuthenticatedController
    {
        private IFormInstanceRuleExecutionLogService _ruleExecutionLogService;
        private IFolderVersionServices _folderVersionServices;
        private IConsumerAccountService _consumerAccountService;
        public FormInstanceRuleExecutionLogController(IFormInstanceRuleExecutionLogService ruleExecutionLogService, IFolderVersionServices folderVersionServices, IConsumerAccountService consumerAccountService)
        {
            this._ruleExecutionLogService = ruleExecutionLogService;
            this._folderVersionServices = folderVersionServices;
            this._consumerAccountService = consumerAccountService;
        }

        public JsonResult GetRuleExecutionLogData(int formInstnaceId, int parentRowID, bool isParentData, string sessionId, GridPagingRequest gridPagingRequest)
        {
            GridPagingResponse<FormInstanceRuleExecutionLogViewModel> ruleExecutionLogDataList = this._ruleExecutionLogService.GetRuleExecutionLogData(formInstnaceId, parentRowID, isParentData, sessionId, gridPagingRequest);

            return Json(ruleExecutionLogDataList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveFormInstanceRuleExecutionLogData(int tenantId, int formInstanceId, int folderId, int folderVersionId, int formDesignId, int formDesignVersionId, string ruleExecutionLogFormInstanceData)
        {
            ServiceResult result = null;
            List<FormInstanceRuleExecutionLogViewModel> loggerDataJsonObject = JsonConvert.DeserializeObject<List<FormInstanceRuleExecutionLogViewModel>>(ruleExecutionLogFormInstanceData);
            result = this._ruleExecutionLogService.SaveFormInstanceRuleExecutionlogData(tenantId, formInstanceId, folderId, folderVersionId, formDesignId, formDesignVersionId, loggerDataJsonObject);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRuleDescription(int ruleID)
        {
            RuleRowModel objRule = this._ruleExecutionLogService.GetRuleDescription(ruleID);

            return Json(objRule, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public ActionResult ExportToExcel(string csv, bool isGroupHeader, int noOfColInGroup, bool isChildGrid,
           string repeaterName, string formName, int folderVersionId, int folderId, int tenantId)
        {
            string header = string.Empty;
            csv = HttpUtility.UrlDecode(csv);
            FolderVersionViewModel folderVersionViewModel = _folderVersionServices.GetFolderVersion(CurrentUserId, CurrentUserName, tenantId, folderVersionId, folderId);


            if (folderVersionViewModel != null)
            {
                if (folderVersionViewModel.AccountId.HasValue && folderVersionViewModel.AccountId.Value > 0)
                {
                    header = "Account:" + _consumerAccountService.GetAccountName(1, folderVersionViewModel.AccountId.Value);
                }
                header = header + "\r\nFolder Name: " + folderVersionViewModel.FolderName;
                header = header + "\r\nVersion Number: " + folderVersionViewModel.FolderVersionNumber;
                if (folderVersionViewModel.EffectiveDate != null && folderVersionViewModel.EffectiveDate != DateTime.MinValue)
                {
                    header = header + "\r\nEffective Date: " + folderVersionViewModel.EffectiveDate.ToString("MM/dd/yyyy");
                }
                header = header + "\r\nDocument Name: " + formName;
                header = header + "\r\nRepeater Name: " + repeaterName;
            }

            ExcelBuilder excelBuilder = new ExcelBuilder();
            MemoryStream fileStream = excelBuilder.ExportToExcel(csv, isGroupHeader, noOfColInGroup, isChildGrid, header);

            var fileDownloadName = repeaterName + ".xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            var fsr = new FileStreamResult(fileStream, contentType);
            fsr.FileDownloadName = fileDownloadName;

            return fsr;
        }

        public JsonResult GetCurrentHTTPSessionID()
        {
            return Json(System.Web.HttpContext.Current.Session["CurrrentSessionId"].ToString(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRuleExecutionServerLogData(int formInstnaceId, string sessionID, int parentRowID, bool isParentData)
        {
            List<FormInstanceRuleExecutionServerLogViewModel> ruleExecutionServerLogDataList = this._ruleExecutionLogService.GetRuleExecutionServerLogData(formInstnaceId, sessionID,parentRowID, isParentData);

            return Json(ruleExecutionServerLogDataList, JsonRequestBehavior.AllowGet);
        }
    }
}