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
using tmg.equinox.applicationservices.viewmodels.exitvalidate;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.domain.entities;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.queueprocess.exitvalidate;
using tmg.equinox.web.extensions;
using tmg.equinox.web.FormInstance;
using tmg.equinox.web.Framework;

namespace tmg.equinox.web.Controllers
{
    public class ExitValidateController : AuthenticatedController
    {
        #region Private Members
        private IExitValidateService _evService;
        private IFormDesignService _formDesignService;
        private IFolderVersionServices _folderVersionService;
        private IFormInstanceDataServices _formInstanceDataService;
        private IUIElementService _uiElementService;
        private IFormInstanceService _formInstanceService;
        private IReportingDataService _reportingDataService;
        private IMasterListService _masterListService;
        private IPBPExportServices _pbpExportServices;
        private IExitValidateEnqueueService _evEnqueueService;
        #endregion

        #region Constructor
        public ExitValidateController(IExitValidateService evService, IExitValidateEnqueueService evEnqueueService)
        {
            this._evService = evService;
            this._evEnqueueService = evEnqueueService;
        }

        public ExitValidateController(IExitValidateService evService, IExitValidateEnqueueService evEnqueueService, IFormDesignService formDesignService, IFolderVersionServices folderVersionServices, IFormInstanceDataServices formInstanceDataServices, IUIElementService uiElementService, IFormInstanceService formInstanceService, IReportingDataService reportingDataService, IMasterListService masterListService, IPBPExportServices pbpExportServices)
        {
            this._evService = evService;
            this._evEnqueueService = evEnqueueService;
            this._formDesignService = formDesignService;
            this._folderVersionService = folderVersionServices;
            this._formInstanceDataService = formInstanceDataServices;
            this._uiElementService = uiElementService;
            this._formInstanceService = formInstanceService;
            this._reportingDataService = reportingDataService;
            this._masterListService = masterListService;
            this._pbpExportServices = pbpExportServices;
        }
        #endregion

        // GET: ExitValidate
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExitValidate()
        {
            //ViewBag.RoleId = RoleID;
            //ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);
            return View("~/Views/FolderVersion/ExitValidate.cshtml");
        }

        public ActionResult GetIsExitValidate()
        {
            ExitValidateViewModel evModel = _evService.GetDefaultSectionData(CurrentUserId);
            return Json(evModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Validate(ExitValidateViewModel model)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                model = _evService.Validate(model);
                ExitValidateViewModel evModel = _evService.GetExitValidateMappings(model.QueueID);
                applicationservices.viewmodels.PBPImport.FormInstanceViewModel formInstance = new applicationservices.viewmodels.PBPImport.FormInstanceViewModel();
                formInstance.FolderVersionId = model.FolderVersionID;
                formInstance.FormInstanceID = model.FormInstanceID;
                formInstance.FormDesignVersionID = model.FormDesignVersionID;
                formInstance.Name = model.Name;

                PBPExportPreProcessor preprocess = new PBPExportPreProcessor(model.QueueID, CurrentUserId, CurrentUserName, _formDesignService, _folderVersionService, _formInstanceDataService, _uiElementService, _formInstanceService, _reportingDataService, _masterListService, formInstance.FormInstanceID);
                preprocess.ExitValidateProcessRulesAndSaveSections(formInstance);
                ExitValidateQueueInfo evQueueInfo = new ExitValidateQueueInfo { QueueId = model.QueueID, AssemblyName = "tmg.equinox.applicationservices", ClassName = "ExitValidateCustomQueue" };
                _evEnqueueService.CreateJob(evQueueInfo);
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                result.Result = ServiceResultStatus.Failure;
                throw ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetExitValidateErrors(int formInstanceId)
        {
            int tenantId = 1;
            //get form instance details
            FormInstanceViewModel model = _folderVersionService.GetFormInstance(tenantId, formInstanceId);
            int pbpViewId = formInstanceId;
            List<ExitValidateResultViewModel> evModels = null;
            List<ExitValidateResultViewModel> resultModels = new List<ExitValidateResultViewModel>();

            if (model.FormDesignID == 2409)
            {
                //VBID View, get the PBP View and then get results
                List<FormInstanceViewModel> models = _folderVersionService.GetFormInstanceList(tenantId, model.FolderVersionID, model.FolderID);
                var pbpModel = from mdl in models where mdl.FormDesignID == 2367 select mdl;
                if (pbpModel != null && pbpModel.Count() > 0)
                {
                    int pbpFormInstanceId = _evService.GetPBPViewFormInstanceID(model.FolderVersionID, formInstanceId);
                    evModels = _evService.GetVBIDViewExitValidateResults(pbpFormInstanceId);
                    var currentVBIDView = models.Where(a => a.FormInstanceID == formInstanceId);
                    if (currentVBIDView != null && currentVBIDView.Count() > 0)
                    {
                        string viewNum = currentVBIDView.First().FormInstanceName.Replace("VBID View ", "");
                        int viewNumber = 0;
                        if (int.TryParse(viewNum, out viewNumber) == true)
                        {
                            string endsWithCheck = '(' + viewNumber.ToString() + ')';
                            string endsWithCheck2 = "Package " + viewNumber.ToString();
                            var filteredModels = from evModel in evModels
                                                 where evModel.Section == "Section B-19" && (evModel.Screen.EndsWith(endsWithCheck) || evModel.Screen.EndsWith(endsWithCheck2))
                                                 select evModel;
                            if (filteredModels != null && filteredModels.Count() > 0)
                            {
                                //filter 19A
                                var models19A = from modA in filteredModels where modA.Screen != null && modA.Screen.ToUpper().Contains("19A") && modA.PBPViewSection !=null && modA.PBPViewSection.ToUpper().Contains("19A") select modA;
                                //filter 19B
                                var models19B = from modB in filteredModels where modB.Screen != null && modB.Screen.ToUpper().Contains("19B") && modB.PBPViewSection != null && modB.PBPViewSection.ToUpper().Contains("19B") select modB;
                                var modelsOther = from modOther in filteredModels where modOther.Screen != null && !modOther.Screen.ToUpper().Contains("19A") && !modOther.Screen.ToUpper().Contains("19B") select modOther;
                                if (models19A != null && models19A.Count() > 0)
                                {
                                    resultModels.AddRange(models19A.ToList());
                                }
                                if (models19B != null && models19B.Count() > 0)
                                {
                                    resultModels.AddRange(models19B.ToList());
                                }
                                if (modelsOther != null && modelsOther.Count() > 0)
                                {
                                    resultModels.AddRange(modelsOther.ToList());
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //map PBP View Sections
                evModels = _evService.GetLatestExitValidateResults(pbpViewId);
                if (evModels != null && evModels.Count() > 0)
                {
                    resultModels = evModels.Where(a => ((a.Section != "Section B-19") ||
                       (a.Section == "Section B-19" && string.IsNullOrEmpty(a.Screen) && string.IsNullOrEmpty(a.Question))) &&
                       (!a.Error.Contains("Are you sure you want to delete Optional Supplemental Benefit Package")) ||
                       (a.Section == "Section B-19" && !string.IsNullOrEmpty(a.Screen) && a.Screen.Equals("#19 VBID/MA Uniformity Flexibility/SSBCI"))).ToList();

                    MapPBPViewSections(evModels);
                }

            }
            return Json(resultModels, JsonRequestBehavior.AllowGet);
        }

        public ActionResult IsExitValidateInProgress(int formInstanceId, int folderVersionId)
        {
            bool result = _evService.IsExitValidateInProgress(formInstanceId, folderVersionId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetExitValidateExportedList()
        {
            IEnumerable<ExitValidateExportedList> evModels = _evService.GetExitValidateExportedList();
            return Json(evModels, JsonRequestBehavior.AllowGet);
        }

        private void MapPBPViewSections(IEnumerable<ExitValidateResultViewModel> evModels)
        {
            List<ExitValidateMapModel> mapModels = _evService.GetSectionMapModels();
            foreach (var model in evModels)
            {
                var mod = from map in mapModels
                          where map.PBPSection == model.Section && (model.Screen != null && model.Screen.StartsWith(map.PBPScreenNumber))
                          select map;
                if (mod != null && mod.Count() > 0)
                {
                    model.PBPViewSection = mod.First().PBPView.Trim();
                }
            }
        }



        public ActionResult ExportExitValidate(string csv, int noOfColInGroup, string repeaterName, int formInstanceId)
        {
            FileStreamResult fsr = null;
            int tenantId = 1;
            //get form instance details
            FormInstanceViewModel model = _folderVersionService.GetFormInstance(tenantId, formInstanceId);
            int pbpViewId = formInstanceId;
            IEnumerable<ExitValidateResultViewModel> evModels = null;
            List<ExitValidateResultViewModel> resultModels = new List<ExitValidateResultViewModel>();

            if (model.FormDesignID == 2409)
            {
                //VBID View, get the PBP View and then get results
                List<FormInstanceViewModel> models = _folderVersionService.GetFormInstanceList(tenantId, model.FolderVersionID, model.FolderID);
                var pbpModel = from mdl in models where mdl.FormDesignID == 2367 select mdl;
                if (pbpModel != null && pbpModel.Count() > 0)
                {
                    int pbpFormInstanceId = _evService.GetPBPViewFormInstanceID(model.FolderVersionID, formInstanceId);
                    evModels = _evService.GetVBIDViewExitValidateResults(pbpFormInstanceId);
                    var currentVBIDView = models.Where(a => a.FormInstanceID == formInstanceId);
                    if (currentVBIDView != null && currentVBIDView.Count() > 0)
                    {
                        string viewNum = currentVBIDView.First().FormInstanceName.Replace("VBID View ", "");
                        int viewNumber = 0;
                        if (int.TryParse(viewNum, out viewNumber) == true)
                        {
                            string endsWithCheck = '(' + viewNumber.ToString() + ')';
                            string endsWithCheck2 = "Package " + viewNumber.ToString();
                            var filteredModels = from evModel in evModels
                                                 where evModel.Section == "Section B-19" && (evModel.Screen.EndsWith(endsWithCheck) || evModel.Screen.EndsWith(endsWithCheck2))
                                                 select evModel;
                            if (filteredModels != null && filteredModels.Count() > 0)
                            {
                                //filter 19A
                                var models19A = from modA in filteredModels where modA.Screen != null && modA.Screen.ToUpper().Contains("19A") && modA.PBPViewSection != null && modA.PBPViewSection.ToUpper().Contains("19A") select modA;
                                //filter 19B
                                var models19B = from modB in filteredModels where modB.Screen != null && modB.Screen.ToUpper().Contains("19B") && modB.PBPViewSection != null && modB.PBPViewSection.ToUpper().Contains("19B") select modB;
                                var modelsOther = from modOther in filteredModels where modOther.Screen != null && !modOther.Screen.ToUpper().Contains("19A") && !modOther.Screen.ToUpper().Contains("19B") select modOther;
                                if (models19A != null && models19A.Count() > 0)
                                {
                                    resultModels.AddRange(models19A.ToList());
                                }
                                if (models19B != null && models19B.Count() > 0)
                                {
                                    resultModels.AddRange(models19B.ToList());
                                }
                                if (modelsOther != null && modelsOther.Count() > 0)
                                {
                                    resultModels.AddRange(modelsOther.ToList());
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //map PBP View Sections
                evModels = _evService.GetLatestExitValidateResults(pbpViewId);
                if (evModels != null && evModels.Count() > 0)
                {
                    resultModels = evModels.Where(a => ((a.Section != "Section B-19") ||
                          (a.Section == "Section B-19" && string.IsNullOrEmpty(a.Screen) && string.IsNullOrEmpty(a.Question))) &&
                          (!a.Error.Contains("Are you sure you want to delete Optional Supplemental Benefit Package")) ||
                          (a.Section == "Section B-19" && !string.IsNullOrEmpty(a.Screen) && a.Screen.Equals("#19 VBID/MA Uniformity Flexibility/SSBCI"))).ToList();

                    MapPBPViewSections(evModels);
                }
            }
            if (resultModels != null && resultModels.Count() > 0)
            {
                DataTable dt = new DataTable();

                dt.Columns.Add("Result Id");
                dt.Columns.Add("Section");
                dt.Columns.Add("Screen");
                dt.Columns.Add("Question");
                dt.Columns.Add("Error");

                foreach (var resultModel in resultModels)
                {
                    DataRow row = dt.NewRow();
                    row["Result Id"] = resultModel.ExitValidateResultID;
                    row["Section"] = resultModel.Section;
                    row["Screen"] = resultModel.Screen;
                    row["Question"] = resultModel.Question;
                    row["Error"] = resultModel.Error;
                    dt.Rows.Add(row);
                }
                ExcelBuilder excelBuilder = new ExcelBuilder();
                MemoryStream fileStream = excelBuilder.DownLoadDataTableToExcel(dt, "Exit Validate Export", "Exit Validate Audit");
                var fileDownloadName = "ExitValidate-Audit" + ".xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                fsr = new FileStreamResult(fileStream, contentType);
                fsr.FileDownloadName = fileDownloadName;
            }
            return fsr;
        }

        public ActionResult CheckExitValidationCompletedForFolderVersion(int folderversionId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                /// Return True - Notify EV run Successfully without error
                /// Return false - Notify EV run Successfully with error
                /// Return null -  No need to sent Notification
                bool? resultval = _evService.CheckEVNotificationSentToUser(folderversionId);
                if (resultval == null)
                {
                    result.Result = ServiceResultStatus.Warning;
                }
                else
                if ((bool)resultval)
                    result.Result = ServiceResultStatus.Success;
                else
                    result.Result = ServiceResultStatus.Failure;
            }
            catch (Exception ex)
            {
                result.Result = ServiceResultStatus.Failure;
                throw ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExitValidateExportDownload(int exitValidateQueueId, string folderVersion)
        {
            string folderpath = string.Empty;
            string userName = User.Identity.Name;
            var queueDetails = _evService.GetExitValidateQueue(exitValidateQueueId);
            string year = folderVersion.Length > 4 ? folderVersion.Substring(0, 4) : "2019";
            string folder = new DirectoryInfo(queueDetails.ExitValidateFilePath).Parent.Name;
            string fileDownloadName = folder + "_" + year + ".zip";
            string zipFilePath = string.Empty;
            try
            {
                folderpath = queueDetails.ExitValidateFilePath.Replace(new DirectoryInfo(queueDetails.ExitValidateFilePath).Name, "");
                zipFilePath = folderpath + fileDownloadName;
                using (ZipFile zip = new ZipFile(zipFilePath))
                {
                    zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                    Response.Clear();
                    Response.BufferOutput = false;
                    Response.ContentType = "application/zip";
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileDownloadName);
                    if (!System.IO.File.Exists(zipFilePath))
                    {
                        foreach (var file in Directory.GetFiles(folderpath))
                        {
                            if (file.EndsWith(".MDB"))
                                zip.AddFile(file, fileDownloadName.Replace(".zip", ""));
                        }
                        zip.Save(zipFilePath);
                    }
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

        [ValidateInput(false)]
        public JsonResult ValidateExitValidationForFolderversion(int folderVersionId)
        {
            ServiceResult result = new ServiceResult();
            var isExitValidationCompleted = _evService.CheckExitValidationCompletedForFolderVersion(folderVersionId);
            if (isExitValidationCompleted)
                result.Result = ServiceResultStatus.Success;
            else
            {
                result.Result = ServiceResultStatus.Warning;
                //_evService.ExitValidateFolderversion(folderVersionId, (Int32)CurrentUserId, CurrentUserName);
                var pbpFormInstancesForTheExitValidation = _evService.GetFormInstancesForEV(folderVersionId);
                //Check formInstance has completed EV
                foreach (var frmInstance in pbpFormInstancesForTheExitValidation)
                {
                    //Check If EV is in progress "Processing"
                    var formInstanceEvStatus = _evService.GetFormInstancesEVStatus(frmInstance.FormInstanceID);
                    if (formInstanceEvStatus == null || ((formInstanceEvStatus.Status != "Processing" && formInstanceEvStatus.Status != "Enqueued") &&
                        ((formInstanceEvStatus.Status == "Succeeded" || formInstanceEvStatus.Status == "Failed") && formInstanceEvStatus.CompletedDate < (frmInstance.UpdatedDate == null ? frmInstance.AddedDate : frmInstance.UpdatedDate))))
                    {
                        result.Result = ServiceResultStatus.Failure;
                        //Queue formInstance for EV
                        ExitValidateViewModel model = new ExitValidateViewModel();
                        model.AddedBy = CurrentUserName;
                        model.AddedDate = DateTime.Now;
                        model.FolderVersionID = folderVersionId;
                        model.FormInstanceID = frmInstance.FormInstanceID;
                        model.FormDesignVersionID = frmInstance.FormDesignVersionID;
                        model.UserID = (Int32)CurrentUserId;
                        model.ProductID = frmInstance.Name;
                        model.Name = frmInstance.Name;
                        model.FormName = "";
                        model.FolderID = frmInstance.FolderVersion.FolderID;
                        model.IsQueuedForWFStateUpdate = true;
                        model.UsersInterestedInStatus = "";
                        var evmodelNew = _evService.Validate(model);
                        ExitValidateViewModel evModel = _evService.GetExitValidateMappings(evmodelNew.QueueID);
                        applicationservices.viewmodels.PBPImport.FormInstanceViewModel formInstance = new applicationservices.viewmodels.PBPImport.FormInstanceViewModel();
                        formInstance.FolderVersionId = evModel.FolderVersionID;
                        formInstance.FormInstanceID = evModel.FormInstanceID;
                        formInstance.FormDesignVersionID = evmodelNew.FormDesignVersionID;
                        formInstance.Name = evModel.Name;

                        PBPExportPreProcessor preprocess = new PBPExportPreProcessor(evModel.QueueID, CurrentUserId, CurrentUserName, _formDesignService, _folderVersionService, _formInstanceDataService, _uiElementService, _formInstanceService, _reportingDataService, _masterListService, formInstance.FormInstanceID);
                        preprocess.ExitValidateProcessRulesAndSaveSections(formInstance);
                        ExitValidateQueueInfo evQueueInfo = new ExitValidateQueueInfo { QueueId = evModel.QueueID, AssemblyName = "tmg.equinox.applicationservices", ClassName = "ExitValidateCustomQueue" };
                        _evEnqueueService.CreateJobWithLowpriority(evQueueInfo);
                        //_evEnqueueService.CreateJob(evQueueInfo);
                    }
                    else
                    {
                        if (formInstanceEvStatus != null && formInstanceEvStatus.UserID != CurrentUserId && ((formInstanceEvStatus.Status == "Processing" || formInstanceEvStatus.Status == "Enqueued") ||
                        ((formInstanceEvStatus.Status == "Succeeded" || formInstanceEvStatus.Status == "Failed") && formInstanceEvStatus.CompletedDate > (frmInstance.UpdatedDate == null ? frmInstance.AddedDate : frmInstance.UpdatedDate))))
                        {
                            //Send notification to the user that plan EV is already completed.
                            _evService.SendEVCompletionotification(formInstanceEvStatus.QueueID, false, (int)CurrentUserId);
                        }
                    }
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}