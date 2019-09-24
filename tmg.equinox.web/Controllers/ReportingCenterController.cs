using System.Collections.Generic;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.WCReport;
using tmg.equinox.web.Framework;
using System.Linq;
using System;
using tmg.equinox.infrastructure.exceptionhandling;
using System.Threading.Tasks;
using tmg.equinox.identitymanagement;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.web.FormInstance;
using System.IO;
using System.Data;
using tmg.equinox.applicationservices.viewmodels.Settings;
using System.Configuration;
using tmg.equinox.reporting.Base.Model;
using tmg.equinox.reporting.Base;
using tmg.equinox.applicationservices;
using tmg.equinox.schema.Base.Model;
using tmg.equinox.dependencyresolution;
using tmg.equinox.repository.interfaces;
using tmg.equinox.config;
using tmg.equinox.emailnotification;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.emailnotification.Model;
using tmg.equinox.applicationservices.ReportingCenter;
using tmg.equinox.repository;
//using tmg.equinox.caching.Interfaces;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.applicationservices.viewmodels.ReportingCenter;
using OfficeOpenXml;
using System.IO;
using System.Web.Script.Serialization;
using tmg.equinox.web.ODMExecuteManager;
using tmg.equinox.setting.Interface;

namespace tmg.equinox.web.Controllers
{
    public class ReportingCenterController : AuthenticatedController
    {
        #region Private Members
        private IReportMasterService _reportService;
        private IReportQueueServices _reportQueueService;
        //private IReportingCenterSchemaService _reportingCenterSchemaService;
        private ISectionLockService _sectionLockService;
        private ISettingManager _settingManager;
        private IMDMSyncDataService _mDMSyncDataService;
        private IFolderVersionServices _folderVersionService;
        #endregion Private Members

        public ReportingCenterController(IReportMasterService reportService, IReportQueueServices reportQueueService, ISectionLockService sectionLockService, ISettingManager settingManager, IMDMSyncDataService mDMSyncDataService, IFolderVersionServices folderVersionService)
        {
            this._reportService = reportService;
            this._reportQueueService = reportQueueService;
            _sectionLockService = sectionLockService;
            _settingManager = settingManager;
            _mDMSyncDataService = mDMSyncDataService;
            _folderVersionService = folderVersionService;
            //this._reportingCenterSchemaService=reportingCenterSchemaService
        }

        [HttpGet]
        public JsonResult GetReportList()
        {
            try
            {
                List<ReportViewModel> reportList = new List<ReportViewModel>();

                reportList = _reportService.GetReportList().ToList();

                return Json(reportList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
        }

        [HttpGet]
        public JsonResult GetReportQueueList(GridPagingRequest gridPagingRequest)
        {
            try
            {
                GridPagingResponse<ReportQueueViewModel> reportQueueList = null;

                reportQueueList = _reportQueueService.GetReportQueueList(gridPagingRequest);

                return Json(reportQueueList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
        }

        public ActionResult AddReportQueue(int ReportId, int[] FolderId, int[] FolderVersionId)
        {
            var Status = "Enqueued";
            int UserId;
            UserId = (CurrentUserId != null) ? CurrentUserId.Value : 0;
            ServiceResult result = this._reportQueueService.AddReportQueue(ReportId, FolderId, FolderVersionId, CurrentUserName, UserId, DateTime.Now, Status);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetReportQueueFolderDetailsList(int queueId)
        {
            try
            {
                List<ReportQueueDetailsViewModel> reportQueueFolderList = null;

                reportQueueFolderList = _reportQueueService.GetReportQueueFolderDetailsList(queueId);

                if (reportQueueFolderList != null && reportQueueFolderList.Count > 0)
                    reportQueueFolderList = _folderVersionService.UpdateReportQueueFolderDetails(reportQueueFolderList);

                return Json(reportQueueFolderList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
        }
        public ActionResult AddSOTQueue(int[] FormInstanceId)
        {
            int[] FolderId = new int[FormInstanceId.Length];
            int[] FolderVersionId = new int[FormInstanceId.Length];
            List<FormInstanceViewModel> formInstanceList = this._reportQueueService.GetFormInstanceDetails(FormInstanceId.ToList());
            int count = 0;
            foreach (FormInstanceViewModel instance in formInstanceList)
            {
                FolderId[count] = instance.FolderID;
                FolderVersionId[count] = instance.FolderVersionID;
                count++;
            }
            var Status = "Enqueued";
            int UserId;
            UserId = (CurrentUserId != null) ? CurrentUserId.Value : 0;
            ServiceResult result = this._reportQueueService.AddReportQueue((int)enumReportType.SOTNonMemberFacingPostBenchmark, FolderId, FolderVersionId, FormInstanceId, CurrentUserName, UserId, DateTime.Now, Status);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: Reporting
        public ActionResult Index()
        {
            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);
            ViewBag.RoleId = RoleID;
            return View();
        }

        // GET: Reporting
        public ActionResult ReportQueue()
        {
            return View();
        }

        public ActionResult ReportingSync()
        {
            return View();
        }

        public ActionResult GetReportSyncDataLog(GridPagingRequest gridPagingRequest)
        {
            GridPagingResponse<ReportingCenterDataSyncViewModel> reportQueueList = null;

            reportQueueList = _mDMSyncDataService.GetReportCenterDataSynclogList(gridPagingRequest);

            return Json(reportQueueList, JsonRequestBehavior.AllowGet);
        }

        // GET: Reporting
        public ActionResult Schema()
        {
            ReportingCenterSchemaService reportingCenterSchemaService = new ReportingCenterSchemaService(new RptUnitOfWork());
            string jsonString = "dataSource: { data: ";
            jsonString += reportingCenterSchemaService.GetReportingSchemaForDisplay();
            jsonString += "}";
            ViewBag.JsonData = jsonString;
            return View();
            //  ReportingCenterSchemaService reportingCenterSchemaService = new ReportingCenterSchemaService(new RptUnitOfWork());
            //  ViewBag.JsonData = reportingCenterSchemaService.GetReportingSchemaForDisplay();
            //  return View();

        }



        //string name,string filepath
        public DownloadResult Download(int ReportQueueId)
        {
            var result = _reportQueueService.GetReportQueueList().ToList().Where(r => r.ReportQueueId.Equals(ReportQueueId)).FirstOrDefault();
            return new DownloadResult
           (
                new DownloadFileInfo()
                {
                    queueId = 1,
                    ReportId = 1,
                    FileName = Path.GetFileName(result.FileName),
                    FilePath = result.DestinationPath
                }
            );
        }

        [HttpPost]
        public FileResult SchemaExport()
        {

            ReportingCenterSchemaService schemaService = new ReportingCenterSchemaService(new RptUnitOfWork());
            List<ReportingTableDetails> list = schemaService.GetRawTableData();

            // DataTable dt = schemaService.ToDataTables(list);


            using (ExcelPackage wb = new ExcelPackage())
            {
                ExcelWorksheet ws = wb.Workbook.Worksheets.Add("Sheet1");
                ws.Cells["A1"].LoadFromCollection<ReportingTableDetails>(list, true);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Schema.xlsx");
                }
            }
        }



        public ActionResult GenerateSchema()
        {
            ServiceResult result = new ServiceResult();
            var _reportingDataService = new ReportingDataService(UnityConfig.Resolve<IRptUnitOfWorkAsync>(), _mDMSyncDataService);
            var generateSchemaService = new GenerateSchemaService(UnityConfig.Resolve<IRptUnitOfWorkAsync>(), _mDMSyncDataService);

            var formDesignService = new FormDesignService(UnityConfig.Resolve<IUnitOfWorkAsync>(), null, null, _reportingDataService, generateSchemaService,_sectionLockService, _settingManager);
            List<JsonDesign> jsonDesigns = formDesignService.GetFormDesignInformation();


            var success = generateSchemaService.Run(jsonDesigns);

            ///Update ReportingCenter Database with existing data
            //List<JData> jData = formDesignService.GetExistingDataForMigration();
            //_reportingDataService.MigrateExistingData(jData);
           
            result.Result = ServiceResultStatus.Success;
           
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PopulateData()
        {
            ServiceResult result = new ServiceResult();
            var _reportingDataService = new ReportingDataService(UnityConfig.Resolve<IRptUnitOfWorkAsync>(), _mDMSyncDataService);
            var generateSchemaService = new GenerateSchemaService(UnityConfig.Resolve<IRptUnitOfWorkAsync>(), _mDMSyncDataService);

            var formDesignService = new FormDesignService(UnityConfig.Resolve<IUnitOfWorkAsync>(), null, null, _reportingDataService, generateSchemaService, _sectionLockService, _settingManager);
             
            ///Update ReportingCenter Database with existing data
            //List<JData> jData = formDesignService.GetExistingDataForMigration();
            //_reportingDataService.MigrateExistingData(jData);
            List<int> forminstanceIds = formDesignService.GetExistingFormInstanceIdsForMigration();
            FolderVersionServices folderVersionServices = new FolderVersionServices(UnityConfig.Resolve<IUnitOfWorkAsync>(), null, _reportingDataService);
            if (forminstanceIds != null)
            {
                foreach (int id in forminstanceIds)
                {
                    folderVersionServices.UpdateReportingCenterDatabase(id, null, false);
                    formDesignService.UpdateFormInstanceIdsOfMigration(id);
                }
            }
               
            result.Result = ServiceResultStatus.Success;
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMDMErrorData(int formInstanceId, int formDesignVersionId)
        {
            var errodata = _mDMSyncDataService.GetMDMErrordata(formInstanceId, formDesignVersionId);

            return Json(errodata, JsonRequestBehavior.AllowGet);
        }


    }
}