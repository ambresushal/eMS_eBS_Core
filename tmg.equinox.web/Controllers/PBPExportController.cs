using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using tmg.equinox.applicationservices;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.dependencyresolution;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.web.extensions;
using tmg.equinox.web.FormInstance;

namespace tmg.equinox.web.Controllers
{
    public class PBPExportController : AuthenticatedController
    {
        #region Private Variables Declaration
        private IPBPExportServices _pbpExportServices;
        private IFormDesignService _formDesignService;
        private IFolderVersionServices _folderVersionService;
        private IFormInstanceDataServices _formInstanceDataService;
        private IUIElementService _uiElementService;
        private IFormInstanceService _formInstanceService;
        private IMasterListService _masterListService;
        private IReportingDataService _reportingDataService;
        private IExportPreQueueService _exportPreQueueService;
        #endregion

        public PBPExportController(IPBPExportServices pbpExportServices, IFormDesignService formDesignService, IFolderVersionServices folderVersionService,IFormInstanceDataServices formInstanceDataService, IUIElementService uiElementService,IFormInstanceService formInstanceService,IMasterListService masterListService,IReportingDataService reportingDataService, IExportPreQueueService exportPreQueueService)
        {
            _pbpExportServices = pbpExportServices;
            _formDesignService = formDesignService;
            _folderVersionService = folderVersionService;
            _formInstanceDataService = formInstanceDataService;
            _uiElementService = uiElementService;
            _formInstanceService = formInstanceService;
            _masterListService = masterListService;
            _reportingDataService = reportingDataService;
            _exportPreQueueService = exportPreQueueService;
        }

        public ActionResult Index()
        {
            ViewBag.RoleId = RoleID;
            return View("~/Views/PBPImport/IndexExport.cshtml");
        }

        public JsonResult GetQueuedPBPExportList(GridPagingRequest gridPagingRequest)
        {
            GridPagingResponse<PBPExportQueueViewModel> pbpExportQueue = null;
            try
            {
                pbpExportQueue = _pbpExportServices.GetQueuedPBPExports(gridPagingRequest);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return Json(pbpExportQueue, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PBPExportDownload(int PBPExportQueueID)
        {
            string folderpath = string.Empty;
            string userName = User.Identity.Name;
            string timeStamp = DateTime.Now.ToString().Replace('/','-').Replace(':','-');
            int ExportYear = _pbpExportServices.GetExportYear(PBPExportQueueID);
            string fileDownloadName = "PBP"+ ExportYear + "IE_" + timeStamp + ".zip";
            string zipFilePath = string.Empty;
            try
            {
                //_pbpExportServices.GenerateMDBFile(PBPExportQueueID, userName);
                folderpath = _pbpExportServices.GetZipFilePath(PBPExportQueueID);
                zipFilePath = folderpath + fileDownloadName;
                using (ZipFile zip = new ZipFile(zipFilePath))
                {
                    zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                    Response.Clear();
                    Response.BufferOutput = false;
                    Response.ContentType = "application/zip";
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileDownloadName);
                    foreach (var file in Directory.GetFiles(folderpath))
                    {
                        if (file.EndsWith(".MDB"))
                            zip.AddFile(file, fileDownloadName.Replace(".zip",""));
                    }
                    zip.Save(zipFilePath);
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return File(zipFilePath, System.Net.Mime.MediaTypeNames.Application.Zip);
        }

        public JsonResult GetPBPDatabaseNames(GridPagingRequest gridPagingRequest)
        {
            GridPagingResponse<PBPImportQueueViewModel> pbpDBs = null;
            try
            {
                pbpDBs = _pbpExportServices.GetDatabaseNamesForPBPExports(gridPagingRequest);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return Json(pbpDBs, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPBPDatabaseDetails(GridPagingRequest gridPagingRequest,int PBPDatabase1Up)
        {
            GridPagingResponse<PBPDatabseDetailViewModel> pbpDBs = null;
            try
            {
                pbpDBs = _pbpExportServices.GetDatabaseDetails(PBPDatabase1Up, gridPagingRequest);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return Json(pbpDBs, JsonRequestBehavior.AllowGet);
        }

        public ActionResult QueuePBPExport(string exportName, string description, string DBName, int pbpDatabase1Up)
        {
            string userName = User.Identity.Name;
            try
            {
                bool scheduleForProcessing = true;
                string preProcessRulesExport = ConfigurationManager.AppSettings["PreProcessRulesExport"] ?? string.Empty;
                if(preProcessRulesExport == "Yes")
                {
                    scheduleForProcessing = false;
                }
                string RunExportRulesInWindowsService = ConfigurationManager.AppSettings["RunExportRulesInWindowsService"] ?? string.Empty;
                ServiceResult result = _pbpExportServices.QueuePBPExport(exportName, description, DBName, userName, pbpDatabase1Up, scheduleForProcessing,CurrentUserId, RunExportRulesInWindowsService);
                if(result.Result == ServiceResultStatus.Success && scheduleForProcessing == false && !RunExportRulesInWindowsService.Equals("Yes"))
                {
                    if(result.Items != null && result.Items.Count() > 0)
                    {
                        string pbpExportQueueIDStr = (result.Items).FirstOrDefault().Messages[0];
                        int pbpExportQueueID = 0;
                        if (!String.IsNullOrEmpty(pbpExportQueueIDStr))
                        {
                            bool parseResult = int.TryParse(pbpExportQueueIDStr, out pbpExportQueueID);
                            if(parseResult == true)
                            {
                                PBPExportPreProcessor exportPreProcessor = new PBPExportPreProcessor(pbpDatabase1Up, pbpExportQueueID, CurrentUserId.Value, CurrentUserName, _pbpExportServices, _formDesignService, _folderVersionService, _formInstanceDataService, _uiElementService, _formInstanceService,_reportingDataService ,_masterListService, _exportPreQueueService,new ExportPreQueueViewModel());
                                Task.Run(() => exportPreProcessor.ProcessRulesAndSaveSections(false));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return Json(true, JsonRequestBehavior.AllowGet); ;
        }

        public ActionResult ExportPBPExport(string csv, int noOfColInGroup, string repeaterName)
        {
            DataTable PBPExportData = _pbpExportServices.GetPBPExportDataTable();

            ExcelBuilder excelBuilder = new ExcelBuilder();
            MemoryStream fileStream = excelBuilder.DownLoadDataTableToExcel(PBPExportData, "PBP Export", "PBP Export Audit");

            var fileDownloadName = "PBPExport-Audit" + ".xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            var fsr = new FileStreamResult(fileStream, contentType);
            fsr.FileDownloadName = fileDownloadName;
            return fsr;
        }
        public JsonResult CheckFolderIsQueued(int folderID)
        {
            bool isQueued = _pbpExportServices.CheckFolderIsQueued(folderID);
            return Json(isQueued, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckExportFolderIsLocked(int pbpDatabase1Up)
        {
            List<ResourceLock> lockedDetails= _pbpExportServices.CheckExportFolderIsLocked(pbpDatabase1Up, CurrentUserId.Value, CurrentUserName);
            return Json(lockedDetails, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetQueuedOrProcessingPBPExport()
        {
            bool result = false;
            PBPExportQueueViewModel model = _pbpExportServices.GetQueuedOrProcessingPBPExport();
            if (model != null)
            {
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}