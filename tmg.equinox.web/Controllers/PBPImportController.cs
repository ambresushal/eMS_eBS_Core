using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
//using tmg.equinox.applicationservices.viewmodels.PBPImport;
//using tmg.equinox.applicationservices.viewmodels.PBPImport;
//using tmg.equinox.pbpimportservices;
using tmg.equinox.applicationservices.viewmodels.Reporting;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.web.FormInstance;
using System.Configuration;
using tmg.equinox.domain.entities.Enums;
using System.Data;
using tmg.equinox.infrastructure.exceptionhandling;
//using tmg.equinox.pbp.Import.processing;
using tmg.equinox.web.Controllers;
using tmg.equinox.web.Framework;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

//using static tmg.equinox.applicationservices.PBPImportService;

namespace tmg.equinox.web.Controllers
{
    public class PBPImportController : AuthenticatedController
    {
        #region Private Variabls Declaration
        private IFolderVersionReportService _folderVersionReportService;
        private IFolderVersionServices _folderVersionServices;
        private IPBPImportService _pBPImportService;
        string IMPORTFILEPATH = ConfigurationManager.AppSettings["PBPImportFiles"].ToString();
        public string PBPTABLENAME = ConfigurationManager.AppSettings["PBPTABLENAME"].ToString();
        public string PBPPLAN_AREASTABLENAME = ConfigurationManager.AppSettings["PBPPLAN_AREASTABLENAME"].ToString();

        #endregion

        #region PBPImport

        public PBPImportController(IFolderVersionReportService _folderVersionReportService, IFolderVersionServices _folderVersionServices, IPBPImportService _pBPImportService)
        {
            this._pBPImportService = _pBPImportService;
            this._folderVersionReportService = _folderVersionReportService;
            this._folderVersionServices = _folderVersionServices;
        }

        public ActionResult Index()
        {
            ViewBag.RoleId = RoleID;
            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);


            return View();
        }

        public JsonResult GetQueuedPBPImportList(GridPagingRequest gridPagingRequest)
        {
            GridPagingResponse<PBPImportQueueViewModel> pbpImportQueueList = null;
            try
            {
                pbpImportQueueList = _pBPImportService.GetPBPImportQueueList(1, gridPagingRequest);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return Json(pbpImportQueueList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadPBPFiles()
        {
            PBPImportResult Result = new PBPImportResult();
            string PBPFileName = string.Empty;
            string PBPPlanAreaFileName = string.Empty;
            if (Request.Files.Count > 0)
            {
                try
                {
                    List<string> FileNameList = SaveFiles(Request);

                    if (FileNameList.Count() > 0)
                    {
                        PBPFileName = Path.GetFileName(FileNameList[0].ToString());
                        PBPPlanAreaFileName = Path.GetFileName(FileNameList[1].ToString());
                    }

                    Result = ValidateAccessFile(FileNameList, PBPFileName, PBPPlanAreaFileName);

                    if (Result.ResultCode == (int)PBPImportErrorCode.Success)
                    {
                        PlanConfigurationViewModel PlanConfigrations = QueuePBPImport(Request, PBPFileName, PBPPlanAreaFileName);
                        return Json(PlanConfigrations);
                    }
                    else
                    {
                        return Json(Result);
                    }
                }
                catch (Exception ex)
                {
                    Result.ResultCode = (int)PBPImportErrorCode.Exception;
                    return Json(Result);
                }
            }
            else
            {
                return Json(0);
            }
        }

        [HttpGet]
        public ActionResult GetLatestInProgressFolderVersionList()
        {
            IEnumerable<ReportingViewModel> folderList = null;
            folderList = _folderVersionReportService.GetFolderList(1, false);
            ReportingViewModel folderVersionList = null;
            List<ReportingViewModel> ResultFolderVersionList = new List<ReportingViewModel>();

            foreach (var item in folderList)
            {
                folderVersionList = _folderVersionReportService.GetFolderVersionList(1, item.FolderId, false).FirstOrDefault();
                ResultFolderVersionList.Add(new ReportingViewModel
                {
                    FolderId = item.FolderId,
                    FolderName = item.FolderName,
                    FolderVersionId = folderVersionList.FolderVersionId,
                    FolderVersionNumber = folderVersionList.FolderVersionNumber
                });
            }

            return Json(ResultFolderVersionList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetMatchAndMisMatchPlanList(int PBPImportQueueID)
        {
            PlanConfigurationViewModel PlanConfigrationLists = _pBPImportService.CreateMatchAndMisMatchPlanLists(PBPImportQueueID);
            return Json(PlanConfigrationLists, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SavePlanConfigurationDetail(int ImportID)
        {
            ServiceResult result = new ServiceResult();
            result = _pBPImportService.SavePBPPlanDetails(ImportID, User.Identity.Name.ToString());
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportToExcel(string csv, int noOfColInGroup, string repeaterName, int PbpImportQueueId,int PlanType)
        {
            try
            {
                DataTable DataTbl = _pBPImportService.GetExportPreviewGridDataTable(PbpImportQueueId, PlanType);
                string header = string.Empty;
                header = "PBP Product Import";
                ExcelBuilder excelBuilder = new ExcelBuilder();

                if (PlanType == 3)
                {
                    header = "PBP Product Import";
                }
                else if (PlanType == 2)
                {
                    header = "PBP Match Plans";
                }
                else if (PlanType == 1)
                {
                    header = "PBP MisMatch Plans";
                }
                MemoryStream fileStream = excelBuilder.DownloadPreviewGridExcel(DataTbl, header, header);
                var fileDownloadName = header.Trim() + ".xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                var fsr = new FileStreamResult(fileStream, contentType);
                fsr.FileDownloadName = fileDownloadName;

                return fsr;
            }
            catch (Exception ex)
            {
                return new FileStreamResult(null, null);
            }
        }

        public ActionResult ExportPBPImportQueuedToExcel(string csv, int noOfColInGroup, string repeaterName)
        {
            string header = string.Empty;
            header = "PBP Import Queue";
            DataTable PBPImportQueuedDt = _pBPImportService.GetExportQueueDataTable();
            ExcelBuilder excelBuilder = new ExcelBuilder();
            MemoryStream fileStream = excelBuilder.DownLoadDataTableToExcel(PBPImportQueuedDt, "PBP Import Queue", "PBP Import Queue");

            var fileDownloadName = "PBPImportQueueList" + ".xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            var fsr = new FileStreamResult(fileStream, contentType);
            fsr.FileDownloadName = fileDownloadName;

            return fsr;
        }

        [HttpPost]
        public JsonResult UpdatePlanConfig(string PBPMatchConfigList)
        {
            IList<PBPMatchConfigViewModel> PBPMatchConfigListobj = JsonConvert.DeserializeObject<IList<PBPMatchConfigViewModel>>(PBPMatchConfigList);
            ServiceResult result = this._pBPImportService.UpdatePlanConfig(1, PBPMatchConfigListobj, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateMatchPlanConfig(string PBPMatchConfigList)
        {
            IList<PBPMatchConfigViewModel> PBPMatchConfigListobj = JsonConvert.DeserializeObject<IList<PBPMatchConfigViewModel>>(PBPMatchConfigList);
            ServiceResult result = this._pBPImportService.UpdateMatchPlanConfig(PBPMatchConfigListobj, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DiscardChanges(int PBPImportQueueID)
        {
            ServiceResult result = this._pBPImportService.DiscardImportChanges(PBPImportQueueID, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region UserAction
        public ActionResult GetUserActionList()
        {
            List<UserActionViewModel> UserActionList = _pBPImportService.GetUserActionList().ToList();
            return Json(UserActionList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region PBPDataBase
        public JsonResult GetPBPDatabaselist(GridPagingRequest gridPagingRequest)
        {
            GridPagingResponse<PBPDatabaseViewModel> PBPDatabaseList = null;
            try
            {
                PBPDatabaseList = _pBPImportService.GetPBPDatabaseList(1, gridPagingRequest);
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
            return Json(PBPDatabaseList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPBPDatabaseNameList()
        {
            List<PBPDatabaseViewModel> PBPDatabaseList = _pBPImportService.GetPBPDatabaseNameList(1);
            return Json(PBPDatabaseList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddPBPDatabase(AddPBPDBNameViewModel AddDB)
        {
            ServiceResult result = this._pBPImportService.AddPBPDatabase(AddDB, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdatePBPDatabase(UpdatePBPDBNameViewModel updateDB)
        {
            ServiceResult result = this._pBPImportService.UpdatePBPDatabase(updateDB, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PBPDataBase()
        {
            ViewBag.RoleId = RoleID;
            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);
            return View();
        }
        #endregion

        #region HelperClass
        private class PBPImportResult
        {
            public int ResultCode { get; set; }
            public string ErrorMsg { get; set; }
        }
        #endregion

        #region Private Method

        private List<string> SaveFiles(HttpRequestBase Request)
        {
            List<string> FileNameList = new List<string>();
            //  Get all files from Request object  
            HttpFileCollectionBase files = Request.Files;
            List<string> FileNamList = new List<string>();
            string FileName, UniqueName = Guid.NewGuid().ToString();
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                // Checking for Internet Explorer  
                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                {
                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                    FileName = testfiles[testfiles.Length - 1];
                }
                else
                {
                    FileName = file.FileName;
                }

                string[] SplitStr = FileName.Split('.');
                string FileUniqueName = SplitStr[0] + UniqueName + "." + SplitStr[1];

                //bool exists = System.IO.Directory.Exists(Server.MapPath(IMPORTFILEPATH));

                //if (!exists)
                //   System.IO.Directory.CreateDirectory(Server.MapPath(IMPORTFILEPATH));

                string fname = Path.Combine(IMPORTFILEPATH, FileUniqueName);
                file.SaveAs(fname);
                FileNameList.Add(FileUniqueName);
                // Get the complete folder path and store the file inside it.  
            }
            return FileNameList;
        }

        private PBPImportResult ValidateAccessFile(List<string> FileNameList, string PBPFileName, string PBPPlanAreaFileName)
        {
            ServiceResult QIDResult = new ServiceResult();
            ServiceResult FileValidateResult = new ServiceResult();
            PBPImportResult PBPImportResult = new PBPImportResult();

            FileValidateResult = _pBPImportService.ValidateFileScheme(PBPFileName, PBPPlanAreaFileName);
            if (FileValidateResult.Result == ServiceResultStatus.Success)
            {
                QIDResult = _pBPImportService.ValidateQID(PBPFileName, PBPPlanAreaFileName);

                if (QIDResult.Result == ServiceResultStatus.Failure)
                {
                    PBPImportResult.ResultCode = (int)PBPImportErrorCode.QIDIsNotSame;
                    string MisMatchQID = string.Empty;
                    foreach (var item in QIDResult.Items)
                    {
                        foreach (var childitem in item.Messages)
                        {
                            MisMatchQID += childitem + " , ";
                            
                        }
                    }
                    if (!string.IsNullOrEmpty(MisMatchQID))
                    {
                        string ostr = MisMatchQID.Remove(MisMatchQID.Length - 2, 1);
                        PBPImportResult.ErrorMsg = "<b>"+ ostr + "</b>";
                    }
                }
                else
                {
                    PBPImportResult.ResultCode = (int)PBPImportErrorCode.Success;
                }
            }
            else
            {
                PBPImportResult.ResultCode = (int)PBPImportErrorCode.FileSchemeIsInvalid;
            }
            return PBPImportResult;
        }

        private PlanConfigurationViewModel QueuePBPImport(HttpRequestBase Request, string PBPFileName, string PBPPlanAreaFileName)
        {
            int PBPImportQueueID = 0;
            string description = Request.Params["description"].ToString();
            string year = Request.Params["year"].ToString();
            string username = User.Identity.Name;
            int Year;
            int.TryParse(Request.Params["year"].ToString(), out Year);
            int PBPDatabase1Up = 0;
            int.TryParse(Request.Params["PBPDatabase1Up"].ToString(), out PBPDatabase1Up);
            string PBPFileDisplayName = String.Empty, PBPPlanAreaFileDisplayName = String.Empty;
            HttpFileCollectionBase files = Request.Files;
            for (int i = 0; i < files.Count; i++)
            {
                if (i == 0)
                {
                    HttpPostedFileBase file = files[i];
                    PBPFileDisplayName = file.FileName.ToString();
                }
                else if (i == 1)
                {
                    HttpPostedFileBase file = files[i];
                    PBPPlanAreaFileDisplayName = file.FileName.ToString();
                }

            }
            PBPImportQueueViewModel pBPImportQueueViewModel = new PBPImportQueueViewModel
            {
                Description = description,
                PBPFileName = PBPFileName,
                PBPPlanAreaFileName = PBPPlanAreaFileName,
                PBPFileDisplayName = PBPFileDisplayName,
                PBPPlanAreaFileDisplayName = PBPPlanAreaFileDisplayName,
                ImportStartDate = DateTime.Now,
                CreatedBy = User.Identity.Name.ToString(),
                CreatedDate = DateTime.Now,
                Status = 0,
                ImportEndDate = DateTime.Now,
                Year = Year,
                PBPDatabase1Up = PBPDatabase1Up
            };
            pBPImportQueueViewModel.Year = _pBPImportService.GetContractYear(pBPImportQueueViewModel.PBPFileName);
            ServiceResult Result = _pBPImportService.AddPBPImportQueue(1, pBPImportQueueViewModel, out PBPImportQueueID);
            List<PBPTableViewModel> PBPTableDetails = _pBPImportService.GetPlanDetailsFromAccessDataBase(null, PBPImportQueueID);
            PlanConfigurationViewModel PlanConfigrations = _pBPImportService.ProcessPlanConfiguration(PBPTableDetails, User.Identity.Name.ToString(), PBPImportQueueID, PBPDatabase1Up);
            PlanConfigrations.PBPImportQueueID = PBPImportQueueID;
            return PlanConfigrations;
        }

        public JsonResult GetQueuedOrProcessingPBPImport()
        {
            bool result = false;
            PBPImportQueueViewModel model = _pBPImportService.GetQueuedOrProcessingPBPImport();
            if (model != null)
            {
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        //public void PBPImportOperationTested(IList<PlanMatchingConfigurationViewModel> PlanConfigurationDetailList)
        //{
        //    int PBPImportQueueID = PlanConfigurationDetailList
        //     .Where(s => s.PBPImportQueueID > 0)
        //     .Select(s => s.PBPImportQueueID)
        //     .FirstOrDefault();

        //    PBPImportEnqueue Enqueue = new PBPImportEnqueue();
        //    Enqueue.Enqueue(new PBPImportQueueInfo {
        //                    QueueId = PBPImportQueueID,
        //                    UserId =this.CurrentUserName });


        //    //ImportProcess Obj = new ImportProcess();
        //    //Obj.Start(PBPImportQueueID);
        //}

    }
}