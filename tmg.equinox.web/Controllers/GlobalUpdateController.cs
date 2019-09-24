using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using tmg.equinox.web.FormInstance;
using System.Data;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.applicationservices.viewmodels.GlobalUpdate;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.applicationservices.viewmodels.GlobalUpdateViewModels;
using Newtonsoft.Json;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using System.Reflection;
using System.Text;
using System.IO;
using System.Configuration;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.web.Framework;
using OfficeOpenXml;
using tmg.equinox.applicationservices.viewmodels;

namespace tmg.equinox.web.Controllers
{
    public class GlobalUpdateController : AuthenticatedController
    {
        #region Private Memebers

        private IUIElementService _uiElementService;
        private IFolderVersionServices _folderVersionServices;
        private IGlobalUpdateService _globalUpdateService;
        private IIASDocumentService _iasDocumentService;
        private int realtimeThresholdLimit;
        private int rollBackHrs;
        public string auditReportPath;
        
        #endregion

        #region Public Properties

        

        #endregion

        #region Constructor
        public GlobalUpdateController(IUIElementService uiElementService, IFolderVersionServices folderVersionServices, IGlobalUpdateService globalUpdateService, IIASDocumentService iasDocumentService)
        {
            this._uiElementService = uiElementService;
            this._folderVersionServices = folderVersionServices;
            this._globalUpdateService = globalUpdateService;
            this._iasDocumentService = iasDocumentService;
        }
        #endregion

        #region Public Method


        public ActionResult Index(string mode, int? rowId)
        {
            var globalUpdateViewModel = new GlobalUpdateViewModel();
            List<IASWizardStepViewModel> IASWizardSteps = _globalUpdateService.GetIASWizardList(1);
            globalUpdateViewModel.iasWizardSteps = IASWizardSteps;
            if (rowId != null)
            {
                var selectedGUData = _globalUpdateService.GetSelectedRowGlobalUpdateData(rowId);
                string globalUpdateName = selectedGUData[0].GlobalUpdateName;
                System.DateTime? effDateFrom = selectedGUData[0].EffectiveDateFrom;
                System.DateTime? effDateTo = selectedGUData[0].EffectiveDateTo;
                int globalUpdateStatusId = selectedGUData[0].GlobalUpdateStatusID;
                int IASWizardStepId = selectedGUData[0].WizardStepsID;
                int globalUpdateId = selectedGUData[0].GlobalUpdateID;

               
                globalUpdateViewModel.EffectiveDateFrom = effDateFrom;
                globalUpdateViewModel.EffectiveDateTo = effDateTo;
                globalUpdateViewModel.GlobalUpdateName = globalUpdateName;
                globalUpdateViewModel.GlobalUpdateStatusID = globalUpdateStatusId;
                globalUpdateViewModel.WizardStepsID = IASWizardStepId;
                globalUpdateViewModel.GlobalUpdateID = globalUpdateId;
               
            }
            return View("Index", globalUpdateViewModel);
        }

        public JsonResult FormDesignVersionList(DateTime effectiveDateFrom, DateTime effectiveDateTo)
        {
            IEnumerable<FormDesignVersionRowModel> formDesignVersionList = _globalUpdateService.GetFormVersions(effectiveDateFrom, effectiveDateTo);
            if (formDesignVersionList == null || formDesignVersionList.Count() == 0)
            {
                formDesignVersionList = new List<FormDesignVersionRowModel>();
            }
            return Json(formDesignVersionList, JsonRequestBehavior.AllowGet);
        }


        public JsonResult FormDesignVersionUIElementList(int tenantId, int formDesignVersionId, int globalUpdateId,bool isOnlySelectedElements)
        {
            IEnumerable<DocumentVersionUIElementRowModel> uiElementList = _globalUpdateService.GetUIElementListForGuFormDesignVersion(tenantId, formDesignVersionId, globalUpdateId, isOnlySelectedElements);
            return Json(uiElementList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckDuplicateFolderVersionExistsInSelectedBatchIAS(string globalUpdateIDArray)
        {
            List<int> guIdList = JsonConvert.DeserializeObject<List<int>>(globalUpdateIDArray);
            ServiceResult serviceResult = this._globalUpdateService.CheckDuplicateFolderVersionExistsInSelectedBatchIAS(guIdList);
            return Json(serviceResult, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveBatch(string batchName, string executionType, DateTime scheduleDate, TimeSpan scheduledTime, string globalUpdateIDArray)
        {
            List<int> guIdList = JsonConvert.DeserializeObject<List<int>>(globalUpdateIDArray);
            InitializeThresholdLimitValue(); 
            ServiceResult serviceResult = this._globalUpdateService.SaveBatch(batchName, executionType, scheduleDate, scheduledTime, DateTime.Now, base.CurrentUserName, guIdList,realtimeThresholdLimit);
            if (serviceResult.Result == ServiceResultStatus.Success)
            {
                var items = serviceResult.Items;
                serviceResult.Items = items;
            }
            return Json(serviceResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateBatch(Guid batchId, string batchName, string executionType, DateTime scheduleDate, TimeSpan scheduledTime, string globalUpdateIDArray)
        {
            List<int> guIdList = JsonConvert.DeserializeObject<List<int>>(globalUpdateIDArray);
            InitializeThresholdLimitValue();
            ServiceResult serviceResult = this._globalUpdateService.UpdateBatch(batchId, batchName, executionType, scheduleDate, scheduledTime, DateTime.Now, base.CurrentUserName, guIdList, realtimeThresholdLimit);
            if (serviceResult.Result == ServiceResultStatus.Success)
            {
                var items = serviceResult.Items;
                serviceResult.Items = items;
            }
            return Json(serviceResult, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ApproveBatch(string batchName)
        {
            ServiceResult serviceResult = this._globalUpdateService.ApproveBatch(batchName, base.CurrentUserName);
            if (serviceResult.Result == ServiceResultStatus.Success)
            {
                var items = serviceResult.Items;
                serviceResult.Items = items;
            }
            return Json(serviceResult, JsonRequestBehavior.AllowGet);
        }
        [NonAction]
        public ActionResult ExistsingGUGrid()
        {
            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);
            ViewBag.RoleId = RoleID;   
            return View();
        }
        [NonAction]
        public ActionResult BatchExecution()
        {
            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);
            ViewBag.RoleId = RoleID;   
            return View();
        }
        [NonAction]
        public ActionResult BatchExecutionStatus()
        {
            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);
            ViewBag.RoleId = RoleID;  
            return View();
        }
        [NonAction]
        [HttpPost]
        public ActionResult AddGlobalUpdate(string globalUpdateName, int globalUpdateId, DateTime effectiveDateFrom, DateTime effectiveDateTo)
        {
            ServiceResult serviceResult = this._globalUpdateService.SaveGlobalUpdate(base.TenantID,globalUpdateId, globalUpdateName, effectiveDateFrom, effectiveDateTo, base.CurrentUserName);
            if (serviceResult.Result == ServiceResultStatus.Success)
            {
                var items = serviceResult.Items;
                serviceResult.Items = items;
            }
            return Json(serviceResult, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]

        public JsonResult GetExistsingGUGrid(GridPagingRequest gridPagingRequest)
        {
            try
            {
                GridPagingResponse<GlobalUpdateViewModel> listExistingGlobalUpdates = _globalUpdateService.GetExistingGlobalUpdatesList(base.TenantID, gridPagingRequest);
                if (listExistingGlobalUpdates == null)
                {
                    List<GlobalUpdateViewModel> emptyGlobalUpdateList = new List<GlobalUpdateViewModel>();
                    listExistingGlobalUpdates = new GridPagingResponse<GlobalUpdateViewModel>(gridPagingRequest.page, 0, gridPagingRequest.rows, emptyGlobalUpdateList);
                }
                return Json(listExistingGlobalUpdates, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
        }
        public JsonResult GetExistsingBatches()
        {
            try
            {
                List<BatchViewModel> listBatches = _globalUpdateService.GetExistingBatchesList(base.TenantID);
                if (listBatches == null)
                {
                    listBatches = new List<BatchViewModel>();
                }
                return Json(listBatches, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
        }
        [HttpGet]

        public JsonResult ExecutedBatch()
        {
            try
            {
                InitializeRollBackHrs();
                List<BatchExecutionViewModel> listExecutedBatches = _globalUpdateService.GetExecutedBatchesList(base.TenantID, rollBackHrs);
                if (listExecutedBatches == null)
                {
                    listExecutedBatches = new List<BatchExecutionViewModel>();
                }
                return Json(listExecutedBatches, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
        }
        [HttpGet]
        public JsonResult GetGUIdsFromBatchMap(Guid batchId)
        {
            try
            {
                List<BatchIASMapViewModel> listAddedIASInBatch = _globalUpdateService.getGUIdsFromBatchMap(batchId);
                if (listAddedIASInBatch == null)
                {
                    listAddedIASInBatch = new List<BatchIASMapViewModel>();
                }
                return Json(listAddedIASInBatch, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
        }

        [HttpGet]
        public JsonResult GetFolderVersionsBaselined(Guid batchId)
        {
            try
            {
                List<GlobalUpateExecutionLogViewModel> listFldrVersionIds = _globalUpdateService.getFolderVersionsBaselined(batchId, CurrentUserId.Value, base.CurrentUserName);
                if (listFldrVersionIds == null)
                {
                    listFldrVersionIds = new List<GlobalUpateExecutionLogViewModel>();
                }
                return Json(listFldrVersionIds, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
        }
        public JsonResult GetImportedNotAddedIASListBatches()
        {
            try
            {
                List<IASElementImportViewModel> listImportedNotAddedIAS = _globalUpdateService.listImportedNotAddedIAS();
                if (listImportedNotAddedIAS == null)
                {
                    listImportedNotAddedIAS = new List<IASElementImportViewModel>();
                }
                return Json(listImportedNotAddedIAS, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
        }
        public JsonResult EditBatchIASListGrid(Guid batchID)
        {
            try
            {
                List<IASElementImportViewModel> ediitBatchIASList = _globalUpdateService.editBatchIASListGrid(batchID);
                if (ediitBatchIASList == null)
                {
                    ediitBatchIASList = new List<IASElementImportViewModel>();
                }
                return Json(ediitBatchIASList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
        }
        public JsonResult ViewBatchIASListGrid(Guid batchID)
        {
            try
            {
                List<IASElementImportViewModel> viewBatchIASList = _globalUpdateService.viewBatchIASListGrid(batchID);
                if (viewBatchIASList == null)
                {
                    viewBatchIASList = new List<IASElementImportViewModel>();
                }
                return Json(viewBatchIASList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
        }
        public JsonResult GetFolderVersionWorkFlowList(int tenantId, int folderVersionId)
        {
            IEnumerable<IASWizardStepViewModel> iasWizardStepsList = _globalUpdateService.GetIASWizardList(tenantId);
            if (iasWizardStepsList == null)
            {
                iasWizardStepsList = new List<IASWizardStepViewModel>();
            }
            return Json(iasWizardStepsList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveElementSelection(int tenantId, int formDesignId, int formDesignVersionId, int globalUpdateId, string selectedUIElementList)
        {
            List<int> uiElementList = JsonConvert.DeserializeObject<List<int>>(selectedUIElementList);
            ServiceResult result = _globalUpdateService.SaveFormDesignVersionUIElements(tenantId, formDesignId, formDesignVersionId, globalUpdateId, uiElementList, base.CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSelectedRowGlobalUpdateData(int globalUpdateId)
        {
            List<GlobalUpdateViewModel> globalUpdateData = _globalUpdateService.GetSelectedRowGlobalUpdateData(globalUpdateId);
            return Json(globalUpdateData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUpdatedDocumentVersions(int globalUpdateId)
        {
            List<FormDesignVersionRowModel> documentVersionList = _globalUpdateService.GetUpdatedDocumentDesignVersion(globalUpdateId);
            return Json(documentVersionList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSelectedUIElementsList(int globalUpdateId, int formDesignVersionId) {
            List<FormDesignElementValueVeiwModel> selectedElementsList = _globalUpdateService.GetSelectedUIElementsList(globalUpdateId, formDesignVersionId);
            return Json(selectedElementsList, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetSelectedDocumentVersionsUIElements(int globalUpdateId, int formDesignVersionId)
        //{
        //    IEnumerable<DocumentVersionUIElementRowModel> uiElementList = _globalUpdateService.GetUIElementListForGuFormDesignVersion(globalUpdateId, formDesignVersionId);
        //    return Json(uiElementList, JsonRequestBehavior.AllowGet);
        //}

        [HttpGet]
        public JsonResult GetIASImpactedFolderVersionList(int GlobalUpdateID, DateTime effectiveDateFrom, DateTime effectiveDateTo, int tenantId)
        {
            //Below funtion used when effective date range applied for design versions
            IEnumerable<IASFolderDataModel> folderVersionList = _globalUpdateService.GetGlobalUpdateImpactedFolderVersionList(GlobalUpdateID, effectiveDateFrom, effectiveDateTo, tenantId);

            return Json(folderVersionList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ScheduleIASUpload(int GlobalUpdateID)
        {
            ServiceResult result = new ServiceResult();

            result = _globalUpdateService.ScheduleIASUpload(GlobalUpdateID, true, User.Identity.Name);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ScheduleGlobalUpdate(int GlobalUpdateID)
        {
            ServiceResult result = new ServiceResult();

            result = _globalUpdateService.ScheduleGlobalUpdate(GlobalUpdateID, true, User.Identity.Name);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveIASFolderDataValues(int GlobalUpdateID, string IASFolderDataString)
        {
            ServiceResult result = new ServiceResult();
            IEnumerable<IASFolderDataModel> IASFolderDataList = JsonConvert.DeserializeObject<IEnumerable<IASFolderDataModel>>(IASFolderDataString);

            result = _globalUpdateService.SaveIASFolderDataValues(GlobalUpdateID, IASFolderDataList, User.Identity.Name);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetElementSelection(int GlobalUpdateID)
        {
            IEnumerable<FormDesignElementValueVeiwModel> globalUpdatesUIElementsList = _globalUpdateService.GetFormDesignVersionUIElements(GlobalUpdateID);

            return Json(globalUpdatesUIElementsList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetGlobalUpdatesIASFolderDataList(int GlobalUpdateID)
        {
            IEnumerable<IASFolderDataModel> globalUpdatesIASFolderDataList = _globalUpdateService.GetGlobalUpdatesFolderDataList(GlobalUpdateID);

            return Json(globalUpdatesIASFolderDataList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveIASElementExportDataValues(int GlobalUpdateID, string IASFolderDataString)
        {
            ServiceResult serviceResult = null;
            IEnumerable<IASFolderDataModel> IASFolderDataList = JsonConvert.DeserializeObject<IEnumerable<IASFolderDataModel>>(IASFolderDataString);

            serviceResult = _globalUpdateService.SaveIASElementExportDataValues(GlobalUpdateID, IASFolderDataList, User.Identity.Name);

            return Json(serviceResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportIASExcelTemplate(int GlobalUpdateID, string GlobalUpdateName, DateTime GlobalUpdateEffectiveDateFrom, DateTime GlobalUpdateEffectiveDateTo)
        {
            var fileDownloadName = GlobalUpdateConstants.IASReportText + GlobalUpdateID + " - " + GlobalUpdateName + GlobalUpdateConstants.ExcelFileExtension;
            var contentType = GlobalUpdateConstants.ExcelContentType;

            //string folderPath = Server.MapPath("\\");
            string folderPath = System.Configuration.ConfigurationManager.AppSettings["IASFilePath"];
            string filePath = string.Empty;
            ServiceResult result = this._iasDocumentService.ExportIASExcelTemplate(GlobalUpdateID, GlobalUpdateName, GlobalUpdateEffectiveDateFrom, GlobalUpdateEffectiveDateTo, folderPath, out filePath);
            return File(filePath, contentType, fileDownloadName);
        }

        [HttpPost]
        public JsonResult CheckForIASExcelTemplate(int GlobalUpdateID, string GlobalUpdateName)
        {
            ServiceResult serviceResult = new ServiceResult();

            var fileDownloadName = GlobalUpdateConstants.IASReportText + GlobalUpdateID + " - " + GlobalUpdateName + GlobalUpdateConstants.ExcelFileExtension;
            
            string folderPath = System.Configuration.ConfigurationManager.AppSettings["IASFilePath"];
            string excelFileFullName = folderPath + fileDownloadName;
            if (System.IO.File.Exists(excelFileFullName))
            {
                serviceResult.Result = ServiceResultStatus.Success;
            }
            else
            {
                serviceResult.Result = ServiceResultStatus.Failure;
            }
            return Json(serviceResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadIASExcelTemplate(int GlobalUpdateID, string GlobalUpdateName, DateTime GlobalUpdateEffectiveDateFrom, DateTime GlobalUpdateEffectiveDateTo)
        {
            var fileDownloadName = GlobalUpdateConstants.IASReportText + GlobalUpdateID + " - " + GlobalUpdateName + GlobalUpdateConstants.ExcelFileExtension;
            var contentType = GlobalUpdateConstants.ExcelContentType;

            string folderPath = System.Configuration.ConfigurationManager.AppSettings["IASFilePath"];
            string excelFileFullName = folderPath + fileDownloadName;

            return File(excelFileFullName, contentType, fileDownloadName);
        }

        [HttpPost]
        public JsonResult CheckForErrorLogExcelTemplate(int GlobalUpdateID, string GlobalUpdateName)
        {
            ServiceResult serviceResult = new ServiceResult();

            var fileDownloadName = GlobalUpdateConstants.ErrorLogReportText + GlobalUpdateID + " - " + GlobalUpdateName + GlobalUpdateConstants.ExcelFileExtension;
            
            string folderPath = System.Configuration.ConfigurationManager.AppSettings["ErrorLogFilePath"];
            string excelFileFullName = folderPath + fileDownloadName;
            if (System.IO.File.Exists(excelFileFullName))
            {
                serviceResult.Result = ServiceResultStatus.Success;
            }
            else
            {
                serviceResult.Result = ServiceResultStatus.Failure;
            }
            return Json(serviceResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadErrorLogExcelTemplate(int GlobalUpdateID, string GlobalUpdateName, DateTime GlobalUpdateEffectiveDateFrom, DateTime GlobalUpdateEffectiveDateTo)
        {
            var fileDownloadName = GlobalUpdateConstants.ErrorLogReportText + GlobalUpdateID + " - " + GlobalUpdateName + GlobalUpdateConstants.ExcelFileExtension;
            var contentType = GlobalUpdateConstants.ExcelContentType;

            string folderPath = System.Configuration.ConfigurationManager.AppSettings["ErrorLogFilePath"];
            string excelFileFullName = folderPath + fileDownloadName;

            return File(excelFileFullName, contentType, fileDownloadName);
        }

        public ActionResult ProcessIASTemplate(int GlobalUpdateID)
        {
            ServiceResult result = new ServiceResult();
            foreach (string file in Request.Files)
            {
                var fileContent = Request.Files[file];
                if (fileContent != null && fileContent.ContentLength > 0)
                {
                    bool isValidFile = IsValidIASFile(GlobalUpdateID, fileContent, result);

                    if (isValidFile)
                    {
                        // get a stream
                        var fileName = Path.GetFileName(file);
                        var templateGuid = Guid.NewGuid() + Path.GetExtension(fileName);
                        // save file 
                        //var path = Path.Combine(Server.MapPath("~/App_Data/IAS"), templateGuid);
                        var path = Path.Combine(System.Configuration.ConfigurationManager.AppSettings["IASFilePath"], templateGuid);
                        fileContent.SaveAs(path);

                        Session["GlobalTemplateGuid"] = templateGuid;

                        IASFileUploadViewModel viewModel = GetIASFileUploadViewModel(fileName, fileContent.ContentType, templateGuid, GlobalUpdateID);
                        result = _globalUpdateService.AddIASTemplate(viewModel);
                    }
                    else
                    {
                        result.Result = ServiceResultStatus.Failure;
                    }
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
                     
        private bool IsValidIASFile(int GlobalUpdateID, HttpPostedFileBase fileContent, ServiceResult result)
        {
            bool isValidFile = false;
            string fileName = fileContent.FileName;
            string fileContentType = fileContent.ContentType;
            byte[] fileBytes = new byte[fileContent.ContentLength];
            var data = fileContent.InputStream.Read(fileBytes, 0, Convert.ToInt32(fileContent.ContentLength));

            try
            {
                using (var package = new ExcelPackage(fileContent.InputStream))
                {
                    foreach (var workSheet in package.Workbook.Worksheets)
                    {
                        isValidFile = false;
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        if (noOfCol > 10 && noOfRow > 3)
                        {
                            int excelGlobalUpdateID = Convert.ToInt32(Convert.ToString(workSheet.Cells[4, 1].Text));
                            if (excelGlobalUpdateID == GlobalUpdateID)
                            {
                                //Check at least one Yes option selected for Accept Changes column in excel
                                isValidFile = CheckYesOptionSelectedInIASFile(noOfRow, noOfCol, workSheet);
                                if (!isValidFile)
                                {
                                    ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem()
                                        {
                                            Messages = new string[] { "Please check at least one 'Yes' option selected for 'Accept Changes' column in '" + workSheet.Name + "' excel sheet!!" }
                                        });
                                    return isValidFile= false;
                                }
                            }
                            else
                            {
                                ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem()
                                {
                                    Messages = new string[] { "Please upload valid Impact Assessment file!!" }
                                });
                                return isValidFile = false;
                            }
                        }
                        else
                        {
                            ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem()
                            {
                                Messages = new string[] { "Please upload valid Impact Assessment file!!" }
                            });
                            return isValidFile = false;
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
            return isValidFile;
        }

        private bool CheckYesOptionSelectedInIASFile(int noOfRow, int noOfCol, ExcelWorksheet workSheet)
        {
            bool isValidFile = false;
            int count = 0;
            for (int r = 4; r <= noOfRow; r++)
            {
                for (int c = 11; c < noOfCol; c++)
                {
                    count++;
                    if (count % 6 == 0)
                    {
                        string value = Convert.ToString(workSheet.Cells[r, c].Text);
                        if (value == GlobalUpdateConstants.YES)
                        {
                            isValidFile = true;
                            return isValidFile;
                        }
                    }
                }
            }
            return isValidFile;
        }

        private IASFileUploadViewModel GetIASFileUploadViewModel(string fileName, string contentType, string templateGuid, int GlobalUpdateID)
        {
            IASFileUploadViewModel viewModel = new IASFileUploadViewModel();
            viewModel.FileName = fileName;
            viewModel.FileExtension = contentType;
            viewModel.TemplateGuid = templateGuid;
            viewModel.GlobalUpdateID = GlobalUpdateID;
            viewModel.AddedBy = base.CurrentUserName;
            viewModel.AddedDate = DateTime.Now;

            return viewModel;
        }

        public ActionResult ValidateIAS(int GlobalUpdateID, string GlobalUpdateName, DateTime GlobalUpdateEffectiveDateFrom, DateTime GlobalUpdateEffectiveDateTo)
        {
            string globalTemplateGuid = Convert.ToString(Session["GlobalTemplateGuid"]);

            //string importPath = Server.MapPath("~/App_Data/IAS/") + globalTemplateGuid;
            string importPath = System.Configuration.ConfigurationManager.AppSettings["IASFilePath"] + globalTemplateGuid;
            DataTable dt = new DataTable();
            //string folderPath = Server.MapPath("\\");
            string folderPath = System.Configuration.ConfigurationManager.AppSettings["ErrorLogFilePath"];
            string filePath = string.Empty;
            ServiceResult result = this._iasDocumentService.ValidateIASExcelTemplate(GlobalUpdateID, GlobalUpdateName, GlobalUpdateEffectiveDateFrom, GlobalUpdateEffectiveDateTo, importPath, base.CurrentUserName, folderPath, out filePath);

            var fileDownloadName = GlobalUpdateConstants.ErrorLogReportText + GlobalUpdateID + " - " + GlobalUpdateName + GlobalUpdateConstants.ExcelFileExtension;
            var contentType = GlobalUpdateConstants.ExcelContentType;
            if (filePath == "")
            {
                return View("ExistsingGUGrid");
            }
            else
            {
                return File(filePath, contentType, fileDownloadName);
            }
        }

        public JsonResult UpdateUIElementValue(GuUpdateUIElementValueViewModel model)
        {
            ServiceResult result = new ServiceResult();
            result = this._globalUpdateService.UpdateValue(model.TenantId, base.CurrentUserName, model.ElementHeader, model.GlobalUpdateId, model.FormDesignVersionId, model.UIElementID, model.UIElementTypeId, model.AreRulesModified, model.Rules, model.IsPropertyGridModified);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Rules(int tenantId, int formDesignVersionId, int uiElementId, int globalUpdateId)
        {
            IEnumerable<GuRuleRowModel> models = _globalUpdateService.GetRulesForUIElement(tenantId, formDesignVersionId, uiElementId, globalUpdateId);
            if (models == null)
            {
                models = new List<GuRuleRowModel>();
            }
            return Json(models, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateValueConfirmationDetails(int uiElementId, int globalUpdateId, int formDesignVersionId)
        {
            ElementHeaderViewModel headerDetails = _globalUpdateService.ConfirmedUpdateValueNotification(uiElementId, globalUpdateId, formDesignVersionId);
            return Json(headerDetails, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUIElemetsForSearchGrid(int tenantId, int formDesignVersionId)
        {
            IEnumerable<DocumentVersionUIElementRowModel> searchGridDetails = _globalUpdateService.GetUpdateSectionUIElements(tenantId, formDesignVersionId);
            return Json(searchGridDetails, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GlobalUpdateBaseLineFolderVersions()
        {
            //List<BaselineDataViewModel> baselineDataList = JsonConvert.DeserializeObject<List<BaselineDataViewModel>>(baselineData);
            List<GlobalUpateExecutionLogViewModel> result = _globalUpdateService.GlobalUpdateBaseLineFolderList(new Guid(),null, 0,  null);
            //GlobalUpdateBaseLineFolder(1,0, 1213, 1326, CurrentUserId.Value, CurrentUserName, "2016_0.03", "Baseline", DateTime.Now,false,false,false);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// RollBack executed batches
        /// </summary>
        /// <param name="batchID"></param>
        /// <returns></returns>

        [HttpPost]
        public ActionResult RollBackBatch(Guid batchID, string rollbackComment)
        {
            ServiceResult serviceResult = null;
            serviceResult = _globalUpdateService.rollBackBatch(batchID, rollbackComment);

            return Json(serviceResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ExecuteBatch(Guid BatchID, string BatchName, int tenantId)
        {
            ServiceResult serviceResult = null;

            serviceResult = _globalUpdateService.ExecuteBatch(BatchID, BatchName, tenantId, User.Identity.Name, CurrentUserId.Value);

            return Json(serviceResult, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult IsValidIASUpload(int GlobalUpdateID)
        {
            try
            {
                bool isExist = _globalUpdateService.IsValidIASUpload(GlobalUpdateID);
                
                return Json(isExist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
        }

        [HttpGet]
        public JsonResult CheckForAuditReport(Guid batchID)
        {
            ServiceResult serviceResult = new ServiceResult();
            GetAuditReportPath();
            string fileFullPath = auditReportPath + batchID + GlobalUpdateConstants.ExcelFileExtension;

            if (System.IO.File.Exists(fileFullPath))
            {
                serviceResult.Result = ServiceResultStatus.Success;
            }
            else
            {
                serviceResult.Result = ServiceResultStatus.Failure;
            }
            return Json(serviceResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public FileResult DownloadAuditReport(Guid batchID)
        {
            GetAuditReportPath();
            string fileFullPath = auditReportPath + batchID + GlobalUpdateConstants.ExcelFileExtension;
            var fileName = batchID + GlobalUpdateConstants.ExcelFileExtension;
            var mimeType = GlobalUpdateConstants.ExcelContentType;
            return File(new FileStream(fileFullPath, FileMode.Open), mimeType, fileName);

        }

        [HttpPost]
        public ActionResult RefreshGlobalUpdateStatus(string inProgressGlobalUpdateIds)
        {
            List<int> globalUpdateIdList = JsonConvert.DeserializeObject<List<int>>(inProgressGlobalUpdateIds);
            List<GlobalUpdateComputedStatus> GetLatestGlobalUpdateStatus = _globalUpdateService.GetLatestGlobalUpdateStatus(globalUpdateIdList);
            return Json(GetLatestGlobalUpdateStatus, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteBatch(Guid batchId)
        {
            ServiceResult serviceResult = null;
            serviceResult = _globalUpdateService.DeleteBatch(batchId,base.CurrentUserName);
            return Json(serviceResult, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult SaveExecutionSchedule(string schedularName, DateTime executionDate, TimeSpan executionTime)
        //{
        //    ServiceResult serviceResult = null;
        //    //serviceResult = _globalUpdateService.DeleteBatch(batchId, base.CurrentUserName);
        //    return Json(serviceResult, JsonRequestBehavior.AllowGet);
        //}

        #endregion

        private void InitializeThresholdLimitValue()
        {

            if (!Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings["RealTimeThreshold"], out realtimeThresholdLimit))
            {
                realtimeThresholdLimit = 20;
            }
        }
        private void InitializeRollBackHrs()
        {

            if (!Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings["RollBackHrs"], out rollBackHrs))
            {
                rollBackHrs = 20;
            }
        }
        private void GetAuditReportPath()
        {
            auditReportPath = System.Configuration.ConfigurationManager.AppSettings["AuditReportFilePath"];

        }

        
    }
}


