using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.ConsumerAccount;
using tmg.equinox.applicationservices.viewmodels.Qhp;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.web.extensions;

namespace tmg.equinox.web.Controllers
{
    public class QHPExportController : AuthenticatedController
    {
        private int _qhpFormDesignID = 2424;
        private int _qhpFormDesignVersionID = 2418;
        private IConsumerAccountService _accountService { get; set; }
        private IQhpIntegrationService _qhpIntegrationService { get; set; }
        private IFormDesignService _formDesignService { get; set; }
        private IFormInstanceService _formInstanceService { get; set; }
        private IFolderVersionServices _folderVersionService;
        private IFormInstanceDataServices _formInstanceDataService;
        private IReportingDataService _reportingDataService;
        private IMasterListService _masterListService;
        private IUIElementService _uiElementService;
        public QHPExportController(IConsumerAccountService accountService, IQhpIntegrationService qhpIntegrationService, IFormDesignService formDesignService, IFormInstanceService formInstanceService, IFolderVersionServices folderVersionService, IFormInstanceDataServices formInstanceDataService, IReportingDataService reportingDataService, IMasterListService masterListService, IUIElementService uiElementService)
        {
            this._accountService = accountService;
            this._qhpIntegrationService = qhpIntegrationService;
            this._formDesignService = formDesignService;
            this._formInstanceService = formInstanceService;
            _folderVersionService = folderVersionService;
            _formInstanceDataService = formInstanceDataService;
            _reportingDataService = reportingDataService;
            _masterListService = masterListService;
            _uiElementService = uiElementService;
        }

        // GET: QHPExport
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetDocumentsList(int tenantId, GridPagingRequest gridPagingRequest)
        {
            GridPagingResponse<DocumentViewModel> documentsList = this._accountService.GetDocumentsListForQHP(tenantId, gridPagingRequest);
            return Json(documentsList, JsonRequestBehavior.AllowGet);
        }

        public FileResult GenerateQHPReport(int tenantId, string targets, bool offExchangeOnly)
        {
            string jsonStringTargets = HttpUtility.UrlDecode(targets);
            List<DocumentViewModel> targetRows = JsonConvert.DeserializeObject<List<DocumentViewModel>>(jsonStringTargets);

            var formInstances = targetRows.Select(s => s.FormInstanceID).ToList();
            int folderVersionID = (int)targetRows.Select(s => s.FolderVersionID).FirstOrDefault();
            var qhpFormInstances = this._formInstanceService.GetQHPViewByAnchor(formInstances, this._qhpFormDesignID, offExchangeOnly);
            var formInstanceList = qhpFormInstances.Select(s => s.FormInstanceId).ToList();

            string folderPath = Server.MapPath("\\");
            string filePath = string.Empty;
            if (formInstanceList.Count() > 0)
            {
                //Execute expression Rules on QHP Documents
                QHPExportPreProcessor exportPreProcessor = new QHPExportPreProcessor(CurrentUserId.Value, CurrentUserName, _qhpIntegrationService, _folderVersionService, _formDesignService, _formInstanceDataService, _reportingDataService, _masterListService, _uiElementService, _formInstanceService);
                exportPreProcessor.ProcessRulesAndSaveSections(formInstanceList);
            }
            ServiceResult result = this._qhpIntegrationService.ExportQhpExcelTemplate(tenantId, folderVersionID, formInstanceList, folderPath, out filePath, offExchangeOnly);
            return System.IO.File.Exists(filePath) ? File(filePath, "application/zip", "Plan And Benefit Package Template.zip") : null;
        }

        public ActionResult UpdateQHPReportQueue(int tenantId, string targets, bool offExchangeOnly)
        {
            string jsonStringTargets = HttpUtility.UrlDecode(targets);
            List<DocumentViewModel> targetRows = JsonConvert.DeserializeObject<List<DocumentViewModel>>(jsonStringTargets);
            ServiceResult result = new ServiceResult();
            try
            {
                QHPReportingQueue queue = _qhpIntegrationService.UpdateQHPReportQueue(targetRows, base.CurrentUserName, offExchangeOnly);
                if (queue != null)
                {
                    var formInstanceList = targetRows.Select(s => s.FormInstanceID).ToList();
                    if (formInstanceList.Count() > 0)
                    {
                        QHPExportPreProcessor exportPreProcessor = new QHPExportPreProcessor(CurrentUserId.Value, CurrentUserName, _qhpIntegrationService, _folderVersionService, _formDesignService, _formInstanceDataService, _reportingDataService, _masterListService, _uiElementService, _formInstanceService);
                        //exportPreProcessor.ProcessRulesAndSaveSections(formInstanceList, queue);
                        Task.Run(() => exportPreProcessor.ProcessRulesAndSaveSections(formInstanceList, queue));

                    }
                }
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                result.Result = ServiceResultStatus.Failure;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetQHPReportQueue(int tenantId, GridPagingRequest gridPagingRequest)
        {
            GridPagingResponse<QHPReportQueueViewModel> documentsList = _qhpIntegrationService.GetQHPReportQueueList(tenantId, gridPagingRequest);
            return Json(documentsList, JsonRequestBehavior.AllowGet);
        }

        public FileResult DownloadQHPReport(int QueueID)
        {
            string filePath = _qhpIntegrationService.GetQHPReportPath(QueueID);
            return System.IO.File.Exists(filePath) ? File(filePath, "application/zip", "Plan And Benefit Package Template.zip") : null;
        }

        public JsonResult GetErrorDescription(int QueueID)
        {
            QHPReportQueueViewModel objQueue = _qhpIntegrationService.GetErrorDescription(QueueID); ;

            return Json(objQueue, JsonRequestBehavior.AllowGet);
        }
    }
}