using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Security.Claims;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.ConsumerAccount;
using tmg.equinox.identitymanagement;
using tmg.equinox.web.Framework;
using System.Drawing.Printing;
using System.Web.Configuration;
using System.IO;
using TuesPechkin;
using System.Net;
using System.Collections.Specialized;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using Newtonsoft.Json;
using System.Configuration;
using System.Net.Mail;
using tmg.equinox.web.Handler;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.FormInstance;
using Newtonsoft.Json.Converters;
using System.Dynamic;
using Newtonsoft.Json.Linq;
using tmg.equinox.applicationservices.viewmodels;
using System.Reflection;
using System.Text;
using Ionic.Zip;
using tmg.equinox.web.Framework.Caching;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.applicationservices.viewmodels.Settings;
using tmg.equinox.backgroundjob;
using tmg.equinox.queueprocess.masterlistcascade;
using tmg.equinox.domain.viewmodels;
using System.Web;
using tmg.equinox.applicationservices.viewmodels.DPF;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.notification;
using System.Threading.Tasks;
using tmg.equinox.setting.Interface;

namespace tmg.equinox.web.Controllers
{
    public class FolderVersionController : AuthenticatedController
    {
        #region Private Members
        private IFolderVersionServices folderVersionService;
        private IWorkFlowStateServices workflowStateService;
        private IConsumerAccountService _consumerAccountService;
        private IJournalReportService _journalReportService;
        private IFormDesignService _formDesignServices;
        private IFolderLockService _folderLockService;
        private IBackgroundJobManager _hangFireJobManager;
        private IMasterListCascadeEnqueueService _mlCascadeQueueService;
        private IMasterListService _masterListService;
        private IMasterListCascadeService _masterListCascadeService;
        private IResourceLockService _resourceLockService;
        private IPBPExportServices _pbpExportServices;
        private IPlanTaskUserMappingService _planTaskUserMappingService;
        private IUIElementService _uielementService;
        private IPBPImportService _pbpImportService;
        private INotificationService _notificationService;
        private ISettingManager _settingManager;
        //TODO: Create a class for constants and move all constants string to that class.
        private const string ACCOUNT = "Account";
        private const string MASTERLIST = "MasterList";
        private const string PRODREF = "PRODREF_";
        private string sendGridUserName = string.Empty;
        private string sendGridPassword = string.Empty;
        private bool _inMemoryFolderLock = false;


        #endregion Private Members

        #region Constructor

        public FolderVersionController(IFolderVersionServices folderVersionService,
            IWorkFlowStateServices workflowStateService, IConsumerAccountService consumerAccountService,
            IJournalReportService journalReportService, IFormDesignService formDesignServices,
            IFolderLockService folderLockService, IBackgroundJobManager hangFireJobManager,
            IMasterListCascadeEnqueueService mlCascadeQueueService, IMasterListService masterListService, IMasterListCascadeService masterListCascadeService, IResourceLockService resourceLockService, IPBPExportServices pbpExportServices,
            IPlanTaskUserMappingService planTaskUserMappingService, IUIElementService uielementService, IPBPImportService pbpImportService, INotificationService notificationService, ISettingManager settingManager)
        {
            this.folderVersionService = folderVersionService;
            this.workflowStateService = workflowStateService;
            this._consumerAccountService = consumerAccountService;
            this._journalReportService = journalReportService;
            this._formDesignServices = formDesignServices;
            this._folderLockService = folderLockService;
            this._hangFireJobManager = hangFireJobManager;
            this._mlCascadeQueueService = mlCascadeQueueService;
            this._masterListService = masterListService;
            this._masterListCascadeService = masterListCascadeService;
            _resourceLockService = resourceLockService;
            this._pbpExportServices = pbpExportServices;
            this._planTaskUserMappingService = planTaskUserMappingService;
            this._uielementService = uielementService;
            this._pbpImportService = pbpImportService;
            this._inMemoryFolderLock = Convert.ToString(ConfigurationManager.AppSettings["FolderLockToUse"]) == "InMemory" ? true : false;
            _notificationService = notificationService;
            _settingManager = settingManager;
        }
        #endregion Constructor

        #region Action Methods

        public ActionResult Index(int tenantId, int? folderVersionId, int? folderId, string foldeViewMode, string folderType = "", bool mode = false, int planTaskId = 0, int navformInstanceId = 0, int navformDesignVersionId = 0, string lockNavInfo = "")
        {
            InitializeViewBags();

            var folderVersionViewModel = FolderVersionViewModel(tenantId, folderVersionId, folderId, ref folderType, mode);
            if (folderVersionViewModel != null)
            {
                folderVersionViewModel.FoldeViewMode = foldeViewMode;
                folderId = folderVersionViewModel.FolderId;
                folderType = folderVersionViewModel.FolderType;
            }
            bool isMasterList = folderVersionService.IsMasterList(folderId.Value);
            if (isMasterList)
            {
                ViewBag.Claims = ControllerHelper.FilterClaimsByMethod(Claims.ToList().Where(c => c.Type.ToUpper().Trim().Contains("/ML")).ToList(), GetActionName);
            }
            else
            {
                ViewBag.Claims = ControllerHelper.FilterClaimsByMethod(Claims.ToList().Where(c => !c.Type.ToUpper().Trim().Contains("/ML")).ToList(), GetActionName);
            }
            ViewBag.accountName = folderVersionService.GetAccountName(folderVersionViewModel.AccountId);
            //Navigate to the Plan, View and selected section if request comes from plan task view (Dashboard)
            if (planTaskId > 0)
            {
                var plantask = _planTaskUserMappingService.GetDPFPlanTaskUserMapping(planTaskId);
                if (plantask != null && plantask.PlanTaskUserMappingDetails != null && JsonConvert.DeserializeObject<PlanTaskUserMappingDetails>(plantask.PlanTaskUserMappingDetails).TaskTraverseDetails != null)
                {
                    var traversedetails = JsonConvert.DeserializeObject<PlanTaskUserMappingDetails>(plantask.PlanTaskUserMappingDetails).TaskTraverseDetails.Split(',');
                    var formDesignVersionId = traversedetails[0].Split(':')[1];
                    var medicareformInstanceId = traversedetails[1].Split(':')[1];
                    //get formInstanceId based on Formdesignversion
                    var formInstanceId = 0;
                    if (String.IsNullOrEmpty(formDesignVersionId))
                    {
                        formInstanceId = Convert.ToInt32(medicareformInstanceId);
                        formDesignVersionId = _planTaskUserMappingService.GetFormDesignVersionByFormInstanceId(formInstanceId).ToString();
                    }
                    else
                    {
                        var data = folderVersionService.GetFormInstanceByAnchorInstanceIdandFormDesignversion(Convert.ToInt32(formDesignVersionId), Convert.ToInt32(medicareformInstanceId));
                        if (data != null)
                            formInstanceId = folderVersionService.GetFormInstanceByAnchorInstanceIdandFormDesignversion(Convert.ToInt32(formDesignVersionId), Convert.ToInt32(medicareformInstanceId)).FormInstanceID;
                    }
                    var sectionId = traversedetails[2].Split(':')[1];
                    TempData["ViewFormInstanceID"] = formInstanceId;
                    if (!String.IsNullOrEmpty(sectionId))
                    {
                        var uielement = _uielementService.GetUIElementByID(Convert.ToInt32(sectionId));
                        if (uielement != null)
                        {
                            TempData["SectionName"] = uielement.UIElementName;
                            TempData["SectionGeneratedName"] = uielement.GeneratedName;
                        }
                    }
                    if (String.IsNullOrEmpty(sectionId) && formInstanceId > 0)
                    {
                        var sectionDesignList = this._planTaskUserMappingService.GetSectionsList(tenantId, formDesignVersionId, this._formDesignServices);
                        if (sectionDesignList != null)
                        {
                            TempData["SectionName"] = (sectionDesignList.FirstOrDefault()).Name;
                            TempData["SectionGeneratedName"] = (sectionDesignList.FirstOrDefault()).GeneratedName;
                        }
                    }
                }
            }
            if (navformInstanceId > 0)
            {
                TempData["ViewFormInstanceID"] = navformInstanceId;
                var sectionDesignList = this._planTaskUserMappingService.GetSectionsList(tenantId, navformDesignVersionId.ToString(), this._formDesignServices);
                if (sectionDesignList != null)
                {
                    TempData["SectionName"] = (sectionDesignList.FirstOrDefault()).Name;
                    TempData["SectionGeneratedName"] = (sectionDesignList.FirstOrDefault()).GeneratedName;
                }
            }
            if (!string.IsNullOrEmpty(lockNavInfo))
            {
                var info = lockNavInfo.Split('_');
                var resourceLockState = _resourceLockService.GetAllLockIntances(base.CurrentUserId.Value).Where(row => row.FormInstanceID == int.Parse(info[0]) && (string.IsNullOrEmpty(row.SectionName) || row.SectionName == info[1])).FirstOrDefault();
                if (resourceLockState != null)
                {
                    var formDesignVersionId = resourceLockState.FormDesignVersionID;
                    var formInstanceId = resourceLockState.FormInstanceID;
                    TempData["ViewFormInstanceID"] = formInstanceId;
                    if (!String.IsNullOrEmpty(resourceLockState.SectionName))
                    {
                        TempData["SectionName"] = resourceLockState.DisplaySectionName;
                        TempData["SectionGeneratedName"] = resourceLockState.SectionName;
                    }
                    if (String.IsNullOrEmpty(resourceLockState.SectionName) && formInstanceId > 0)
                    {
                        var sectionDesignList = this._planTaskUserMappingService.GetSectionsList(tenantId, resourceLockState.FormDesignVersionID.ToString(), this._formDesignServices);
                        if (sectionDesignList != null)
                        {
                            TempData["SectionName"] = (sectionDesignList.FirstOrDefault()).Name;
                            TempData["SectionGeneratedName"] = (sectionDesignList.FirstOrDefault()).GeneratedName;
                        }
                    }
                }
            }
            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["Grid.UseJQGrid"]) == true)
                return View("Index", "FormRenderLayout", folderVersionViewModel);
            else
                return View("Index", "PQFormRenderLayout", folderVersionViewModel);
        }

        public ActionResult IndexML(int tenantId, int? folderVersionId, int? folderId, string folderType = "", bool mode = false)
        {
            InitializeViewBags();

            var folderVersionViewModel = FolderVersionViewModel(tenantId, folderVersionId, folderId, ref folderType, mode);
            if (folderVersionViewModel != null)
            {
                folderId = folderVersionViewModel.FolderId;
                ViewBag.folderType = folderType;
                folderType = folderVersionViewModel.FolderType;
            }
            bool isMasterList = folderVersionService.IsMasterList(folderId.Value);
            if (isMasterList)
            {
                ViewBag.Claims = ControllerHelper.FilterClaimsByMethod(Claims.ToList().Where(c => c.Type.ToUpper().Trim().Contains("ML")).ToList(), GetActionName);
                folderVersionViewModel.FolderType = "MasterList";
            }
            else
            {
                ViewBag.Claims = ControllerHelper.FilterClaimsByMethod(Claims.ToList().Where(c => !c.Type.ToUpper().Trim().Contains("/ML")).ToList(), GetActionName);
            }
            ViewBag.accountName = folderVersionService.GetAccountName(folderVersionViewModel.AccountId);

            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["Grid.UseJQGrid"]) == true)
            {
                ViewBag.IsJQ = true;
                return View("IndexML", "FormRenderLayout", folderVersionViewModel);
            }
            else
            {
                ViewBag.IsJQ = false;
                return View("IndexML", "PQFormRenderLayout", folderVersionViewModel);
            }
        }

        public ActionResult GetMasterListDocuments()
        {
            MasterListDocuments documents = new MasterListDocuments();

            documents = folderVersionService.GetMasterListDocuments();
            return Json(documents, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewEditMLFolderVersion(int tenantId, int folderVersionId, int folderId,
           string folderType = "", bool mode = false)
        {
            InitializeViewBags();

            var folderVersionViewModel = LoadFolderVersionViewModel(tenantId, folderVersionId, folderId, ref folderType, mode);
            bool isMasterList = folderVersionService.IsMasterList(folderId);
            if (isMasterList)
            {
                ViewBag.Claims = ControllerHelper.FilterClaimsByMethod(Claims.ToList().Where(c => c.Type.ToUpper().Trim().Contains("/ML")).ToList(), "Index");
            }
            else
            {
                ViewBag.Claims = ControllerHelper.FilterClaimsByMethod(Claims.ToList().Where(c => !c.Type.ToUpper().Trim().Contains("/ML")).ToList(), GetActionName);
            }
            ViewBag.accountName = folderVersionService.GetAccountName(folderVersionViewModel.AccountId);

            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["Grid.UseJQGrid"]) == true)
                return View("Index", "FormRenderLayout", folderVersionViewModel);
            else
                return View("Index", "PQFormRenderLayout", folderVersionViewModel);

        }

        public ActionResult GetNonPortfolioBasedAccountFolders(int tenantId, int? folderVersionId, int? folderId, string foldeViewMode,
            string folderType = "", bool mode = false)
        {
            InitializeViewBags();

            var folderVersionViewModel = FolderVersionViewModel(tenantId, folderVersionId, folderId, ref folderType, mode);
            if (folderVersionViewModel != null)
            {
                folderVersionViewModel.FoldeViewMode = foldeViewMode;
                folderId = folderVersionViewModel.FolderId;
                folderType = folderVersionViewModel.FolderType;

            }

            bool isMasterList = folderVersionService.IsMasterList(folderId.Value);
            if (isMasterList)
            {
                ViewBag.Claims = ControllerHelper.FilterClaimsByMethod(Claims.ToList().Where(c => c.Type.ToUpper().Trim().Contains("/ML")).ToList(), "Index");
            }
            else
            {
                ViewBag.Claims = ControllerHelper.FilterClaimsByMethod(Claims.ToList().Where(c => !c.Type.ToUpper().Trim().Contains("/ML")).ToList(), GetActionName);
            }
            ViewBag.accountName = folderVersionService.GetAccountName(folderVersionViewModel.AccountId);

            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["Grid.UseJQGrid"]) == true)
                return View("Index", "FormRenderLayout", folderVersionViewModel);
            else
                return View("Index", "PQFormRenderLayout", folderVersionViewModel);

        }

        public ActionResult GetNonPortfolioBasedAccountFoldersML(int tenantId, int? folderVersionId, int? folderId, string foldeViewMode,
           string folderType = "", bool mode = false)
        {
            InitializeViewBags();

            var folderVersionViewModel = FolderVersionViewModel(tenantId, folderVersionId, folderId, ref folderType, mode);
            if (folderVersionViewModel != null)
            {
                folderVersionViewModel.FoldeViewMode = foldeViewMode;
                folderId = folderVersionViewModel.FolderId;
                folderType = folderVersionViewModel.FolderType;

            }
            bool isMasterList = folderVersionService.IsMasterList(folderId.Value);
            if (isMasterList)
            {
                ViewBag.Claims = ControllerHelper.FilterClaimsByMethod(Claims.ToList(), "IndexML");
            }
            else
            {
                ViewBag.Claims = ControllerHelper.FilterClaimsByMethod(Claims.ToList().Where(c => !c.Type.ToUpper().Trim().Contains("/ML")).ToList(), GetActionName);
            }
            ViewBag.accountName = folderVersionService.GetAccountName(folderVersionViewModel.AccountId);

            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["Grid.UseJQGrid"]) == true)
                return View("IndexML", "FormRenderLayout", folderVersionViewModel);
            else
                return View("IndexML", "PQFormRenderLayout", folderVersionViewModel);

        }
        private FolderVersionViewModel LoadFolderVersionViewModel(int tenantId, int folderVersionId, int folderId,
          ref string folderType, bool mode)
        {
            {
                FolderVersionViewModel model = null;
                model = folderVersionService.LoadFolderVersionViewModel(folderId, folderVersionId);


                if (model != null)
                {
                    var inProgress = folderVersionService.IsMasterListFolderVersionInProgress(model.FolderVersionId);

                    folderId = model.FolderId;
                    folderVersionId = model.FolderVersionId;
                    folderType = model.FolderType;
                    model.FolderType = string.Empty;
                    mode = inProgress;
                }
            }

            bool isMasterList = folderVersionService.IsMasterList(folderId);

            FolderVersionViewModel folderVersionViewModel = folderVersionService.GetFolderVersion(CurrentUserId, CurrentUserName, tenantId, folderVersionId,
                                     folderId);
            if (folderVersionViewModel == null)
            {
                folderVersionViewModel = new FolderVersionViewModel();
            }
            //TODO: Replace this with legacy code. There may be a better way of Implementation
            //set the Page Mode Edit Or Delete
            //if mode is true - user can edit the page
            folderVersionViewModel.IsEditable = mode;
            folderVersionViewModel.FolderType = isMasterList ? MASTERLIST : ACCOUNT;
            folderVersionViewModel.IsReleased = !folderVersionService.IsMasterListFolderVersionInProgress(folderVersionViewModel.FolderVersionId);
            folderVersionViewModel.CurrentUserId = CurrentUserId;
            folderVersionViewModel.CurrentUserName = CurrentUserName;
            folderVersionViewModel.RoleId = RoleID;

            //Get the folder lock status
            var enableFolderLockSettings = IdentityManager.IsFolderLockEnable.ToString().ToLower();
            if (mode == true && Convert.ToBoolean(enableFolderLockSettings))
            {
                //FolderVersionViewModel folderlockmodel = folderVersionService.GetFolderLockStatus(tenantId, folderId, CurrentUserId);
                FolderVersionViewModel folderlockmodel = _inMemoryFolderLock ? _folderLockService.GetFolderLockStatus(folderId, CurrentUserId) : folderVersionService.GetFolderLockStatus(tenantId, folderId, CurrentUserId);

                if (folderlockmodel != null)
                {
                    folderVersionViewModel.IsLocked = folderlockmodel.IsLocked;
                    folderVersionViewModel.LockedBy = folderlockmodel.LockedBy;
                    folderVersionViewModel.LockedByUser = folderlockmodel.LockedByUser;

                    if (folderlockmodel.IsLocked == true)
                    {
                        folderVersionViewModel.IsEditable = false;
                    }
                }
                else
                {
                    //Check whether the folder is MasterList
                    if (!isMasterList)
                    {
                        //ServiceResult result = folderVersionService.UpdateFolderLockStatus(CurrentUserId, tenantId, folderId);
                        ServiceResult result = _inMemoryFolderLock ? _folderLockService.UpdateFolderLockStatus(CurrentUserId, folderId) : folderVersionService.UpdateFolderLockStatus(CurrentUserId, tenantId, folderId);
                        if (result.Result == ServiceResultStatus.Failure)
                        {
                            folderVersionViewModel.IsEditable = false;
                            folderVersionViewModel.IsLocked = true;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(folderType) && folderType.Contains(PRODREF))
            {
                string[] prodref = folderType.Split('_');
                folderVersionViewModel.ReferenceProductFormInstanceID = Convert.ToInt32(prodref[1]);
            }
            else
            {
                folderVersionViewModel.ReferenceProductFormInstanceID = 0;
            }
            return folderVersionViewModel;
        }

        public JsonResult GetAllFormsStatus(int folderVersionId, string folderType)
        {
            Dictionary<int, ProductState> productStateList = folderVersionService.GetProductStateList(folderVersionId, folderType);
            return Json(productStateList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPltformPlnDesign(int forminstanceId)
        {
            Dictionary<int, string> pltformPlnDesign = folderVersionService.GetPltformPlnDesign(forminstanceId);
            return Json(pltformPlnDesign, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFolderVersionViewModel(string folderType)
        {

            FolderVersionViewModel model = FolderVersionViewModel(1, null, null, ref folderType, false) ?? new FolderVersionViewModel();
            model.AccountName = folderVersionService.GetAccountName(model.AccountId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        private FolderVersionViewModel FolderVersionViewModel(int tenantId, int? folderVersionId, int? folderId,
            ref string folderType, bool mode)
        {
            //if (!String.IsNullOrEmpty(folderType))

            //    folderId = folderVersionService.GetFolderIdByFolderName(folderType);
            //folderId = folderVersionService.GetAllFolderList(1).Where(s => s.FolderName.Equals(folderType)).Select(s => s.FolderId).FirstOrDefault();
            //{
            FolderVersionViewModel model = null;
            if (folderId.HasValue || folderVersionId.HasValue)
            {
                model = folderVersionService.LoadFolderVersionViewModel(folderId.Value, folderVersionId.Value);  //get latest folderversionId and folderId for Products. 
            }
            else
            {
                model = folderVersionService.GetCurrentFolderVersionML(folderType);             //get latest folderversionId and folderId for Master List.
                if (model != null)
                {
                    var inProgress = folderVersionService.IsMasterListFolderVersionInProgress(model.FolderVersionId);

                    folderId = model.FolderId;
                    folderVersionId = model.FolderVersionId;
                    folderType = model.FolderType;
                    model.FolderType = string.Empty;
                    mode = inProgress;
                }
            }

            FolderVersionViewModel folderVersionViewModel = new FolderVersionViewModel();
            if (folderId != null)
            {
                bool isMasterList = folderVersionService.IsMasterList(folderId.Value);

                folderVersionViewModel = folderVersionService.GetFolderVersion(CurrentUserId, CurrentUserName, tenantId, folderVersionId.Value,
                                         folderId.Value);

                //TODO: Replace this with legacy code. There may be a better way of Implementation
                //set the Page Mode Edit Or Delete
                //if mode is true - user can edit the page
                folderVersionViewModel.IsEditable = mode;
                folderVersionViewModel.FolderType = isMasterList ? MASTERLIST : ACCOUNT;
                folderVersionViewModel.IsReleased = !folderVersionService.IsMasterListFolderVersionInProgress(folderVersionViewModel.FolderVersionId);
                folderVersionViewModel.CurrentUserId = CurrentUserId;
                folderVersionViewModel.CurrentUserName = CurrentUserName;
                folderVersionViewModel.RoleId = RoleID;

                //Get the folder lock status
                //var enableFolderLockSettings = IdentityManager.IsFolderLockEnable.ToString().ToLower();
                //if (mode == true && Convert.ToBoolean(enableFolderLockSettings))
                //{
                //    //FolderVersionViewModel folderlockmodel = folderVersionService.GetFolderLockStatus(tenantId, folderId, CurrentUserId);
                //    FolderVersionViewModel folderlockmodel = _inMemoryFolderLock ? _folderLockService.GetFolderLockStatus(folderId, CurrentUserId) : folderVersionService.GetFolderLockStatus(tenantId, folderId, CurrentUserId);

                //    if (folderlockmodel != null)
                //    {
                //        folderVersionViewModel.IsLocked = folderlockmodel.IsLocked;
                //        folderVersionViewModel.LockedBy = folderlockmodel.LockedBy;
                //        folderVersionViewModel.LockedByUser = folderlockmodel.LockedByUser;

                //        if (folderlockmodel.IsLocked == true)
                //        {
                //            folderVersionViewModel.IsEditable = false;
                //        }
                //    }
                //    else
                //    {
                //        //Check whether the folder is MasterList
                //        if (!isMasterList)
                //        {
                //            //ServiceResult result = folderVersionService.UpdateFolderLockStatus(CurrentUserId, tenantId, folderId);
                //            ServiceResult result = _inMemoryFolderLock ? _folderLockService.UpdateFolderLockStatus(CurrentUserId, folderId) : folderVersionService.UpdateFolderLockStatus(CurrentUserId, tenantId, folderId);
                //            if (result.Result == ServiceResultStatus.Failure)
                //            {
                //                folderVersionViewModel.IsEditable = false;
                //                folderVersionViewModel.IsLocked = true;
                //            }
                //        }
                //    }
                //}

                if (!string.IsNullOrEmpty(folderType) && folderType.Contains(PRODREF))
                {
                    string[] prodref = folderType.Split('_');
                    folderVersionViewModel.ReferenceProductFormInstanceID = Convert.ToInt32(prodref[1]);
                }
                else
                {
                    folderVersionViewModel.ReferenceProductFormInstanceID = 0;
                }
            }
            return folderVersionViewModel;
        }

        public JsonResult GetApprovalStatusTypeList(int tenantId, int folderVersionId, int wfVersionStateId)
        {
            IEnumerable<KeyValue> approvalStatus = workflowStateService.GetApprovalStatusTypeForFolderVersion(tenantId, folderVersionId, wfVersionStateId);
            if (approvalStatus == null)
            {
                approvalStatus = new List<KeyValue>();
            }
            return Json(approvalStatus, JsonRequestBehavior.AllowGet);
        }

        //[AllowAnonymous]
        public JsonResult GetFormInstanceList(int tenantId, int folderVersionId, int folderId, int formDesignTypeId = 0)
        {
            IEnumerable<FormInstanceViewModel> formInstanceList = folderVersionService.GetFormInstanceList(tenantId, folderVersionId, folderId, formDesignTypeId);
            if (formInstanceList == null)
            {
                formInstanceList = new List<FormInstanceViewModel>();
            }
            if (formInstanceList.Count() > 0)
            {
                IDataCacheHandler handler = DataCacheHandlerFactory.GetHandler();
                foreach (var fI in formInstanceList)
                {
                    handler.Remove(fI.FormInstanceID, base.CurrentUserId);
                }
            }
            return Json(formInstanceList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAncherFormInstanceList(int tenantId, int folderVersionId, string folderViewMode)
        {
            IEnumerable<OpenDocumentViewModel> openDocumentList = folderVersionService.GetAncherFormInstanceList(folderVersionId, folderViewMode, base.CurrentUserId);

            if (openDocumentList == null)
            {
                openDocumentList = new List<OpenDocumentViewModel>();
            }

            foreach (var openDoc in openDocumentList)
            {
                foreach (var view in openDoc.DocumentViews)
                {
                    if (view.FormDesignID == 2409)
                    {
                        view.FormDesignDisplayName = view.FormInstanceName;
                    }
                }
            }
            return Json(openDocumentList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDocumentViewList(int tenantId, int anchorFormInstanceId)
        {
            IEnumerable<DocumentViewListViewModel> documentViewList = folderVersionService.GetDocumentViewList(tenantId, anchorFormInstanceId);
            if (documentViewList == null)
            {
                documentViewList = new List<DocumentViewListViewModel>();
            }
            foreach (var view in documentViewList)
            {
                if (view.FormDesignID == 2409)
                {
                    view.FormDesignDisplayName = view.FormInstanceName;
                }
            }

            return Json(documentViewList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUpdatedDocumentList(int tenantId, int folderVersionId)
        {
            IEnumerable<FormInstanceViewModel> updatedDocumentList = folderVersionService.GetUpdatedDocumentList(tenantId, folderVersionId);
            if (updatedDocumentList == null)
            {
                updatedDocumentList = new List<FormInstanceViewModel>();
            }

            return Json(updatedDocumentList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFormInstancesList(int tenantId, List<int> formInstanceIDs, List<DocumentOverrideSelection> unlockDocumentSelections = null)
        {
            IEnumerable<FormInstanceViewModel> updatedDocumentList = folderVersionService.GetFormInstancesList(tenantId, formInstanceIDs);
            if (updatedDocumentList == null)
            {
                updatedDocumentList = new List<FormInstanceViewModel>();
            }
            foreach (FormInstanceViewModel model in updatedDocumentList)
            {
                if (unlockDocumentSelections != null)
                {
                    foreach (DocumentOverrideSelection form in unlockDocumentSelections)
                    {
                        if (model.FormInstanceID == form.FormInstanceId)
                        {
                            if (form.IsDocumentLocked == "true")
                                model.IsFormInstanceEditable = form.IsOverrideDocument;
                            else
                                model.IsFormInstanceEditable = true;
                        }
                    }
                }
            }
            return Json(updatedDocumentList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSOTViewFormInstancesList(int tenantId, List<FolderViewModel> documentList, List<DocumentOverrideSelection> unlockDocumentSelections = null)
        {
            IEnumerable<FormInstanceViewModel> updatedDocumentList = folderVersionService.GetSOTViewFormInstancesList(tenantId, documentList);
            if (updatedDocumentList == null)
            {
                updatedDocumentList = new List<FormInstanceViewModel>();
            }
            foreach (FormInstanceViewModel model in updatedDocumentList)
            {
                if (unlockDocumentSelections != null)
                {
                    foreach (DocumentOverrideSelection form in unlockDocumentSelections)
                    {
                        if (model.FormInstanceID == form.FormInstanceId)
                        {
                            if (form.IsDocumentLocked == "true")
                                model.IsFormInstanceEditable = form.IsOverrideDocument;
                            else
                                model.IsFormInstanceEditable = true;
                        }
                    }
                }
            }
            return Json(updatedDocumentList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetReferenceFormInstance(int tenantId, int folderVersionId, int folderId, int referenceformInstanceId)
        {
            FormInstanceViewModel formInstanceList = folderVersionService.GetFormInstance(tenantId, referenceformInstanceId);
            if (formInstanceList == null)
            {
                formInstanceList = new FormInstanceViewModel();
            }
            if (formInstanceList != null)
            {
                IDataCacheHandler handler = DataCacheHandlerFactory.GetHandler();

                handler.Remove(formInstanceList.FormInstanceID, base.CurrentUserId);

            }
            return Json(formInstanceList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddApplicableTeams(WorkFlowStateFolderVersionMapViewModel model)
        {
            ServiceResult result = new ServiceResult();
            if (model.ApplicableTeamsIDList != null)
            {
                result = this.workflowStateService.AddApplicableTeams(model.ApplicableTeamsIDList.OrderBy(i => i).ToList(), model.FolderID, model.FolderVersionID, CurrentUserName);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckMilestoneChecklistSection(int folderVersionId, string sectionName)
        {
            bool result = this.workflowStateService.CheckMilestoneChecklistSection(folderVersionId, sectionName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApplicableTeams(int folderVersionId)
        {
            IEnumerable<WorkFlowStateFolderVersionMapViewModel> applicableTeamList = workflowStateService.GetApplicableTeams(folderVersionId);
            if (applicableTeamList == null)
            {
                applicableTeamList = new List<WorkFlowStateFolderVersionMapViewModel>();
            }
            return Json(applicableTeamList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetVersionHistory(int folderId, int tenantId, string versionType)
        {
            IEnumerable<FolderVersionHistoryViewModel> models = folderVersionService.GetVersionHistory(folderId, tenantId, versionType);
            if (models == null)
            {
                models = new List<FolderVersionHistoryViewModel>();
            }
            return Json(models, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVersionHistoryML(int folderId, int tenantId, string versionType)
        {
            IEnumerable<FolderVersionHistoryViewModel> models = folderVersionService.GetVersionHistoryML(folderId, tenantId, versionType);
            if (models == null)
            {
                models = new List<FolderVersionHistoryViewModel>();
            }
            return Json(models, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMajorFolderVersionList(int folderId, int tenantId, string userName, string versionNumber,
                                            DateTime effectiveDate)
        {
            IEnumerable<FolderVersionViewModel> majorFolderVersionList = folderVersionService.GetMajorFolderVersionList(tenantId,
                folderId, userName, versionNumber, effectiveDate);
            if (majorFolderVersionList == null)
            {
                majorFolderVersionList = new List<FolderVersionViewModel>();
            }
            return Json(majorFolderVersionList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMinorFolderVersionList(int folderId, int tenantId, string ownerName, bool isBaseLine)
        {
            IEnumerable<FolderVersionViewModel> minorFolderVersionList = folderVersionService.GetMinorFolderVersionList(tenantId, folderId, ownerName, CurrentUserName, isBaseLine);
            if (minorFolderVersionList == null)
            {
                minorFolderVersionList = new List<FolderVersionViewModel>();
            }
            return Json(minorFolderVersionList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult BaseLineFolderVersion(int tenantId, int folderId, int folderVersionId, string versionNumber,
            string comments, DateTime? effectiveDate, bool isRelease)
        {
            isRelease = false;
            ServiceResult result = folderVersionService.BaseLineFolder(tenantId, 0, folderId, folderVersionId, CurrentUserId.Value,
                CurrentUserName, versionNumber, comments, 0, effectiveDate, isRelease, isNotApproved: false, isNewVersion: false);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetMasterFolderVersion(int formDesignId, int tenantId, int? folderVersionId, int? folderId, string folderType = "", bool mode = false)
        {
            InitializeViewBags();
            var folderVersionViewModel = FolderVersionViewModel(tenantId, folderVersionId, folderId, ref folderType, mode);
            if (folderVersionViewModel != null)
            {
                folderId = folderVersionViewModel.FolderId;
                folderType = folderVersionViewModel.FolderType;
            }
            bool isMasterList = folderVersionService.IsMasterList(folderId.Value);
            if (isMasterList)
            {
                ViewBag.Claims = ControllerHelper.FilterClaimsByMethod(Claims.ToList().Where(c => c.Type.ToUpper().Trim().Contains("/ML")).ToList(), GetActionName);
            }
            else
            {
                ViewBag.Claims = ControllerHelper.FilterClaimsByMethod(Claims.ToList().Where(c => !c.Type.ToUpper().Trim().Contains("/ML")).ToList(), GetActionName);
            }
            ViewBag.accountName = folderVersionService.GetAccountName(folderVersionViewModel.AccountId);

            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["Grid.UseJQGrid"]) == true)
                return View("Index", "FormRenderLayout", folderVersionViewModel);
            else
                return View("Index", "PQFormRenderLayout", folderVersionViewModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="folderId"></param>
        /// <returns></returns>
        /// 
        public JsonResult OverrideFolderLock(int tenantId, int folderId)
        {
            //ServiceResult result = folderVersionService.OverrideFolderLock(CurrentUserId, TenantID, folderId);
            ServiceResult result = _inMemoryFolderLock ? _folderLockService.OverrideFolderLock(folderId, CurrentUserId) : folderVersionService.OverrideFolderLock(CurrentUserId, TenantID, folderId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult OverrideDocumentLock(int tenantId, int folderId, List<int> formInstanceIDs)
        {
            ServiceResult result = null;
            List<DocumentViewListViewModel> viewList = new List<DocumentViewListViewModel>();
            //Get all views of Document from selected view
            foreach (int formInstanceId in formInstanceIDs)
                viewList.AddRange(folderVersionService.GetDocumentViewList(tenantId, formInstanceId));

            //List of formInstanceIDs of all views
            formInstanceIDs = viewList.Select(c => c.FormInstanceId).ToList();

            if (_inMemoryFolderLock)
                result = _resourceLockService.OverrideDocumentLock(folderId, CurrentUserId, formInstanceIDs);

            if (result.Result == ServiceResultStatus.Success)
                _resourceLockService.UpdateDocumentLockStatus(folderId, CurrentUserId, formInstanceIDs, base.CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        public JsonResult CheckFolderLockIsOverriden(int folderId, int folderVersionId)
        {
            string result = "true";
            //bool isFolderOverriden = folderVersionService.CheckFolderLockIsOverriden(folderId, CurrentUserId);
            bool isFolderOverriden = _inMemoryFolderLock ? _folderLockService.IsFolderLocked(folderId, CurrentUserId) : folderVersionService.CheckFolderLockIsOverriden(folderId, CurrentUserId);

            if (isFolderOverriden)
            {
                ODMCacheHandler handler = new ODMCacheHandler();
                string data = handler.IsExistsOpenFolderVersion(folderVersionId);
                if (data != null)
                {
                    result = "ODMInProgress";
                    handler.RemoveOpenFolderVersion(folderVersionId);
                }
            }
            else
            {
                result = "false";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckFolderIsQueued(int folderID, int folderVersionID)
        {
            bool isQueued = false;
            ServiceResult result = new ServiceResult();
            ODMCacheHandler handler = new ODMCacheHandler();
            string data = handler.IsExists(folderVersionID);
            if (data != null)
            {
                isQueued = true;
                result.Result = ServiceResultStatus.Success;
                ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Migration process has started on this folder. Folder will be locked for editing until migration process is complete. ODM screen can be referenced for status of migration." } });
            }
            else
            {
                isQueued = _pbpExportServices.CheckFolderIsQueued(folderID);
                if (isQueued)
                {
                    result.Result = ServiceResultStatus.Success;
                    ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "The folder cannot be edited since its plan(s) are being exported. Please open the folder in view mode." } });
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckDocumentLockIsOverriden(int formInstanceId, int formDesignId, string sectionName)
        {
            bool isFolderOverriden = false;
            if (_inMemoryFolderLock)
                isFolderOverriden = _resourceLockService.IsDocumentLocked(formInstanceId, CurrentUserId, sectionName, formDesignId, true);
            if (isFolderOverriden == true)
            {
                //check for ML Cascade
                MasterListCascadeBatchDetailViewModel model = _masterListCascadeService.GetInProgressMatchCascade(formInstanceId);
                if (model != null)
                {
                    if (model.PreviousFormInstanceID == formInstanceId)
                    {
                        isFolderOverriden = false;
                    }
                }

            }
            return Json(isFolderOverriden, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="folderId"></param>
        /// <returns></returns>
        public JsonResult GetFolderLockStatus(int tenantId, int? folderId)
        {
            FolderVersionViewModel result = folderVersionService.GetFolderLockStatus(tenantId, folderId, CurrentUserId);
            if (result != null && result.IsLocked != true)
            {
                result = _inMemoryFolderLock ? _folderLockService.GetFolderLockStatus(folderId, CurrentUserId) : folderVersionService.GetFolderLockStatus(tenantId, folderId, CurrentUserId);
            }
            if (result != null && result.IsLocked == true)
            {
                return Json(result.LockedByUser, JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDocumentLockStatus(int tenantId, int? folderId, List<int> formInstanceIDs)
        {
            List<FolderVersionViewModel> result = new List<FolderVersionViewModel>();
            List<DocumentViewListViewModel> viewList = new List<DocumentViewListViewModel>();
            string documentLock = "false";
            //Get all views of Document from selected view
            foreach (int formInstanceId in formInstanceIDs)
            {
                var isLockingEnabled = _resourceLockService.CheckIfFormInstanceasLockingEnabled(formInstanceId);
                if (!isLockingEnabled)
                    viewList.AddRange(folderVersionService.GetDocumentViewList(tenantId, formInstanceId));
            }

            //List of formInstanceIDs of all views
            formInstanceIDs = viewList.Where(c => c.IsSectionLockEnabled == false).Select(c => c.FormInstanceId).ToList();

            if (_inMemoryFolderLock)
            {
                result = _resourceLockService.GetDocumentLockStatus(folderId, CurrentUserId, formInstanceIDs).ToList();
                if (result != null && result.Count == 0)
                {
                    _resourceLockService.UpdateDocumentLockStatus(folderId, CurrentUserId, formInstanceIDs, base.CurrentUserName);
                }
                else if (result != null && result[0].LockedBy != CurrentUserId)
                {
                    documentLock = "true";
                }

                //foreach (FolderVersionViewModel res in result)
                //{ 
                //    if (res != null && res.IsLocked == true)
                //        lst.Add(res);
                //}
                //if (lst.Count > 0)
                //return Json(lst, JsonRequestBehavior.AllowGet);
            }
            return Json(documentLock, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReleaseDocumentLock(int tenantId, int formInstanceId)
        {
            ServiceResult result = new ServiceResult();
            List<DocumentViewListViewModel> viewList = new List<DocumentViewListViewModel>();

            if (formInstanceId != 0)
            {
                //Get all views of Document from selected view
                viewList.AddRange(folderVersionService.GetDocumentViewList(tenantId, formInstanceId));

                //List of formInstanceIDs of all views
                List<int> formInstanceIDs = viewList.Select(c => c.FormInstanceId).ToList();

                if (_inMemoryFolderLock)
                    result = _resourceLockService.ReleaseDocumentLockOnTabClose(formInstanceIDs, CurrentUserId);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReleaseDocumentLockonOnViewChange(int formInstanceId, int? newFormInstanceId)
        {
            ServiceResult result = new ServiceResult();
            List<DocumentViewListViewModel> viewList = new List<DocumentViewListViewModel>();
            var isLockingEnabled = _resourceLockService.CheckIfFormInstanceasLockingEnabled(formInstanceId);
            if (!isLockingEnabled)
            {
                viewList.AddRange(folderVersionService.GetDocumentViewList(1, formInstanceId));
                List<int> formInstanceIDs = viewList.Where(c => c.IsSectionLockEnabled == false).Select(c => c.FormInstanceId).ToList();
                if (!(newFormInstanceId > 0 && formInstanceIDs.Contains((Int32)newFormInstanceId)))
                {
                    result = _resourceLockService.ReleaseDocumentLockOnTabClose(formInstanceIDs, CurrentUserId);
                }
            }
            else
            {
                result = _resourceLockService.ReleaseDocumentLockOnTabClose(new List<int>() { formInstanceId }, CurrentUserId);
            }

            //List of formInstanceIDs of all views


            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// SH This method is used to check whether a logged in user is a team manager.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public JsonResult CheckUserIsTeamManager()
        {
            bool isTeamManager = folderVersionService.IsTeamManager(CurrentUserId);
            return Json(isTeamManager, JsonRequestBehavior.AllowGet);
        }

        #region Version Management

        [HttpPost]
        public JsonResult AddMinorFolderVersion(int tenantId, int folderId, int folderVersionId, string versionNumber, string comments, DateTime? effectiveDate, bool isRelease, Nullable<int> consortiumID, Nullable<int> categoryID, string catID)
        {
            ServiceResult result = null;
            result = folderVersionService.CreateNewMinorVersion(tenantId, folderId, folderVersionId, versionNumber, comments,
                                    effectiveDate, isRelease, CurrentUserId.Value, consortiumID, categoryID, catID, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsAnyFolderVersionInProgress(int folderId, int tenantId)
        {
            bool isReleased = folderVersionService.IsAnyFolderVersionInProgress(folderId, tenantId);

            return Json(isReleased, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method returns a collection of Data from 'FolderVersion' table.
        /// </summary>
        ///<param name="tenantId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetImpactedFolderVersionList(int folderId, DateTime effectiveDate, int tenantId)
        {
            IEnumerable<RetroChangeViewModel> folderVersionList = folderVersionService.GetImpactedFolderVersionList(folderId,
                                                                        effectiveDate, tenantId);
            return Json(folderVersionList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///GetFolderVersion record details for folderVersionID 
        ///If isCopyRetro = true, pass newEffectiveDate = folderVersionId's effectiveDate and set the newFolderVersion number 
        ///If isCopyRetro = false, pass newEffectiveDate = given effectievDate and set the newFolderVersion number 
        ///Get FolderVersion Number and FolderVersionId from given folderId
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult FolderVersionRetro(List<RetroChangeViewModel> retroChangesList, int folderId, int tenantId,
                DateTime retroEffectiveDate, Nullable<int> categoryID, string catID)
        {
            ServiceResult serviceResult = null;

            if (retroChangesList != null && retroChangesList.Any())
            {
                var folderVersionList = this.folderVersionService.GetVersionNumberForAccountRetroChanges(tenantId, folderId);

                serviceResult = folderVersionService.FolderVersionRetroChanges(retroChangesList, folderVersionList, retroEffectiveDate,
                    categoryID, catID, User.Identity.Name, CurrentUserId.Value);
            }

            return Json(serviceResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Deletes Folder Version records and it's successors
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="folderVersionId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteFolderVersion(int tenantId, int folderVersionId, int folderId, string versionType)
        {
            ServiceResult serviceResult = null;

            //Delete FolderVersion which has Status ="In Progress"
            serviceResult = folderVersionService.DeleteFolderVersion(tenantId, folderId, folderVersionId, versionType, CurrentUserName);

            if (serviceResult.Result == ServiceResultStatus.Success)
            {
                //Set the latest FolderVersion Status from "BaseLined" to "In Progress"
                var items = serviceResult.Items;
                serviceResult = folderVersionService.ChangeFolderVersionStatus(tenantId, folderId, CurrentUserName);
                serviceResult.Items = items;
            }
            return Json(serviceResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ReleaseMLVersion(int tenantId, int folderVersionId, int folderId, string versionNumber)
        {
            ServiceResult serviceResult = null;

            //Release FolderVersion which has Status ="In Progress"
            string comment = string.Empty;
            serviceResult = folderVersionService.ReleaseMLVersion(tenantId, folderId, folderVersionId, CurrentUserName, versionNumber, comment);
            return Json(serviceResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CascadeReleaseMLVersion(int tenantId, int folderVersionId, int folderId, string versionNumber)
        {
            ServiceResult serviceResult = null;
            //check for existing Master List Cascade
            MasterListCascadeBatchViewModel model = _masterListCascadeService.GetQueuedBatch();
            tmg.equinox.applicationservices.viewmodels.PBPImport.PBPImportQueueViewModel importModel = _pbpImportService.GetQueuedOrProcessingPBPImport();
            tmg.equinox.applicationservices.viewmodels.PBPImport.PBPExportQueueViewModel exportModel = _pbpExportServices.GetQueuedOrProcessingPBPExport();

            //check for locked Folders
            if (model == null && importModel == null && exportModel == null)
            {
                List<string> lockedFolderMesssages = GetLockedFoldersForMLCascade(folderVersionId);
                if (lockedFolderMesssages == null || lockedFolderMesssages.Count == 0)
                {
                    FolderVersionViewModel viewmodel = folderVersionService.GetFolderVersionById(folderVersionId);
                    //Release FolderVersion which has Status ="In Progress"
                    string comment = string.IsNullOrEmpty(viewmodel.Comments) ? "Cascade : Masterlist has been released" : "Cascade : " + viewmodel.Comments;
                    MasterListFormDesignViewModel designModel = _masterListService.GetFolderVersionFormDesign(folderVersionId);
                    List<MasterListCascadeViewModel> mlCascades = _masterListCascadeService.GetMasterListCascade(designModel.FormDesignID, designModel.FormDesignVersionID);

                    serviceResult = folderVersionService.ReleaseMLVersion(tenantId, folderId, folderVersionId, CurrentUserName, versionNumber, comment);
                    if (serviceResult.Result == ServiceResultStatus.Success)
                    {
                        //get the FormDesignID and FormDesignVersionID
                        if (mlCascades != null && mlCascades.Count > 0)
                        {
                            int masterListCascadeBatchID = _masterListCascadeService.AddMasterListCascadeBatch(mlCascades[0], CurrentUserName);
                            //queue for processing in the Master List
                            MasterListCascadeQueueInfo mlQueueInfo = new MasterListCascadeQueueInfo { FormDesignID = designModel.FormDesignID, FormDesignVersionID = designModel.FormDesignVersionID, FolderVersionID = folderVersionId, MasterListCascadeBatchID = masterListCascadeBatchID, UserID = CurrentUserId != null ? CurrentUserId.Value : 0, UserName = CurrentUserName };
                            _mlCascadeQueueService.CreateJob(mlQueueInfo);
                        }
                    }
                }
                else
                {
                    serviceResult = new ServiceResult();
                    List<ServiceResultItem> siList = new List<ServiceResultItem>();
                    serviceResult.Result = ServiceResultStatus.Failure;
                    ServiceResultItem item = new ServiceResultItem();
                    string errorMsg = "Master List Cascade cannot be queued since one or more Folders are being edited as below:<br/>";
                    foreach (var msg in lockedFolderMesssages)
                    {
                        errorMsg = errorMsg + msg + "<br/>";
                    }
                    item.Messages = new string[] { errorMsg };
                    siList.Add(item);
                    serviceResult.Items = siList;
                }
            }
            else
            {
                serviceResult = new ServiceResult();
                List<ServiceResultItem> siList = new List<ServiceResultItem>();
                serviceResult.Result = ServiceResultStatus.Warning;
                ServiceResultItem item = new ServiceResultItem();
                if (model != null)
                {
                    item.Messages = new string[] { "A Master List Cascade is already queued or being Processed. Please try again later. Visit the Master List Cascade status screen to check for Status." };
                }
                else if (importModel != null)
                {
                    item.Messages = new string[] { "A PBP Import is already queued or being Processed. Please try again later. Visit the PBP Import Screen to check the status of the Import that is queued or being Processed." };
                }
                else if (exportModel != null)
                {
                    item.Messages = new string[] { "A PBP Export is already queued or being Processed. Please try again later. Visit the PBP Export Screen to check the status of the Export that is queued or being Processed." };
                }

                siList.Add(item);
                serviceResult.Items = siList;
            }

            //lock the Master List 
            return Json(serviceResult, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This method will be called if there are users working in folders for which ML cascade is scheduled.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="folderVersionId"></param>
        /// <param name="folderId"></param>
        /// <param name="versionNumber"></param>
        /// <returns></returns>
        public JsonResult CascadeMLVersion(int tenantId, int folderVersionId, int folderId, string versionNumber)
        {
            SendMessageToUsersOfLockedFoldersForMLCascade(folderVersionId);
            //Task.Delay(300000).Wait();
            ServiceResult serviceResult = null;
            FolderVersionViewModel viewmodel = folderVersionService.GetFolderVersionById(folderVersionId);
            //Release FolderVersion which has Status ="In Progress"
            string comment = string.IsNullOrEmpty(viewmodel.Comments) ? "Cascade : Masterlist has been released" : "Cascade : " + viewmodel.Comments;
            MasterListFormDesignViewModel designModel = _masterListService.GetFolderVersionFormDesign(folderVersionId);
            List<MasterListCascadeViewModel> mlCascades = _masterListCascadeService.GetMasterListCascade(designModel.FormDesignID, designModel.FormDesignVersionID);

            serviceResult = folderVersionService.ReleaseMLVersion(tenantId, folderId, folderVersionId, CurrentUserName, versionNumber, comment);
            if (serviceResult.Result == ServiceResultStatus.Success)
            {
                //get the FormDesignID and FormDesignVersionID
                if (mlCascades != null && mlCascades.Count > 0)
                {
                    int masterListCascadeBatchID = _masterListCascadeService.AddMasterListCascadeBatch(mlCascades[0], CurrentUserName);
                    //queue for processing in the Master List
                    MasterListCascadeQueueInfo mlQueueInfo = new MasterListCascadeQueueInfo { FormDesignID = designModel.FormDesignID, FormDesignVersionID = designModel.FormDesignVersionID, FolderVersionID = folderVersionId, MasterListCascadeBatchID = masterListCascadeBatchID, UserID = CurrentUserId != null ? CurrentUserId.Value : 0, UserName = CurrentUserName };
                    _mlCascadeQueueService.CreateJobWithDelay(mlQueueInfo);
                }
            }
            //lock the Master List 
            return Json(serviceResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult IsFolderVersionCanRollback(int tenantId, string rollbackFolderVersionNumber,
                                              string inProgressMinorVersionNumber)
        {
            ServiceResult serviceResult = null;

            serviceResult = folderVersionService.CanRollbackFolderVersion(tenantId, rollbackFolderVersionNumber,
                                                                            inProgressMinorVersionNumber);

            return Json(serviceResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// passing the folderVersionId to Rollback it's properties
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="folderVersionId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RollbackFolderVersion(int tenantId, int rollbackFolderVersionId, int folderId,
            string rollbackFolderVersionNumber)
        {
            ServiceResult serviceResult = null;

            FolderVersionViewModel inProgressMinorVersion = folderVersionService.GetLatestMinorFolderVersion(tenantId, folderId);

            serviceResult = folderVersionService.RollbackFolderVersion(tenantId, rollbackFolderVersionId, folderId,
                                                        rollbackFolderVersionNumber,
                                                        inProgressMinorVersion, CurrentUserId.Value, CurrentUserName);

            return Json(serviceResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsValidRetroEffectiveDate(int folderID, int tenantID, DateTime retroEffectiveDate)
        {
            ServiceResult serviceResult = null;

            serviceResult = folderVersionService.IsValidRetroEffectiveDate(folderID, tenantID,
                                                                            retroEffectiveDate);

            return Json(serviceResult, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Create Form

        /// <summary>
        /// Gets the form type list to create a new form instance.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        public JsonResult GetFormTypeList(int tenantId, string folderType, DateTime effectiveDate, int folderId)
        {
            IEnumerable<FormTypeViewModel> formTypeList = folderVersionService.GetFormTypeList(tenantId, folderType, effectiveDate, folderId);
            if (formTypeList == null)
            {
                formTypeList = new List<FormTypeViewModel>();
            }
            return Json(formTypeList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the folder list form which the form instances will be copied.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="accountType">Type of the account.</param>
        /// <returns></returns>
        public JsonResult GetFolderList(int tenantId, int accountType, int accountId, int categoryId, bool isFoundation =  false)
        {
            IEnumerable<FolderVersionViewModel> folderList = null;
            if (accountType == 1 || accountType == 3)
            {
                folderList = folderVersionService.GetFolderList(tenantId, true, accountId, categoryId, isFoundation);
            }
            else if (accountType == 2)
            {
                folderList = folderVersionService.GetFolderList(tenantId, false, accountId, categoryId, RoleID, isFoundation);
            }
            if (folderList == null)
            {
                folderList = new List<FolderVersionViewModel>();
            }
            return Json(folderList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the folder list form which the form instances will be copied.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="accountType">Type of the account.</param>
        /// <returns></returns>
        public JsonResult GetAllFoldersList()
        {
            IEnumerable<FolderVersionViewModel> folderList = null;

            folderList = folderVersionService.GetAllFoldersList();

            List<ReportingFolderViewModel> uniqueFolders = new List<ReportingFolderViewModel>();
            List<int> folders = new List<int>();
            foreach (var folder in folderList)
            {
                if (!folders.Contains(folder.FolderId))
                {
                    uniqueFolders.Add(new ReportingFolderViewModel() { FolderId = folder.FolderId, FolderName = folder.FolderName });
                    folders.Add(folder.FolderId);
                }
            }

            List<ReportingFolderViewModel> reportingFoldersData = new List<ReportingFolderViewModel>();
            foreach (var item in uniqueFolders)
            {
                var folderVersions = (from fld in folderList
                                      where fld.FolderId == item.FolderId
                                      select new ReportingFolderVersionViewModel
                                      {
                                          FolderVersionId = fld.FolderVersionId,
                                          FolderVersionNumber = fld.FolderVersionNumber
                                      }).ToList();


                reportingFoldersData.Add(new ReportingFolderViewModel()
                {
                    FolderId = item.FolderId,
                    FolderName = item.FolderName,
                    FolderVersions = folderVersions
                });
            }

            if (reportingFoldersData == null)
            {
                reportingFoldersData = new List<ReportingFolderViewModel>();
            }
            return Json(reportingFoldersData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the folder list form which the form instances will be copied.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="accountType">Type of the account.</param>
        /// <returns></returns>
        public JsonResult GetAllFoldersListPaging(GridPagingRequest gridPagingRequest)
        {
            GridPagingResponse<FolderVersionViewModel> folderList = null;

            folderList = folderVersionService.GetAllFoldersList(gridPagingRequest);

            //if (folderList == null)
            //{
            //    folderList = new GridPagingResponse<FolderVersionViewModel>(gridPagingRequest);
            //}
            return Json(folderList, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Gets the form list to be copied.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="folderVersionId">The folder version identifier.</param>
        /// <returns></returns>
        public JsonResult GetFormList(int tenantId, int folderVersionId)
        {
            IEnumerable<FormInstanceViewModel> formTypeList = folderVersionService.GetFormList(tenantId, folderVersionId);
            if (formTypeList == null)
            {
                formTypeList = new List<FormInstanceViewModel>();
            }
            return Json(formTypeList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getFormInstanceIDList(int tenantId, int folderVersionId)
        {
            List<FormInstanceViewModel> formTypeList = folderVersionService.GetProductList(tenantId, folderVersionId);
            List<int> formInstanceIdList = new List<int>();
            foreach (var a in formTypeList)
            {
                formInstanceIdList.Add(a.FormInstanceID);
            }
            return Json(formInstanceIdList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Saves the form instance.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="folderVersionId">The folder version identifier.</param>
        /// <param name="formDesignVersionId">The form design version identifier.</param>
        /// <param name="formInstanceId">The form instance identifier.</param>
        /// <param name="isCopy">if set to <c>true</c> [is copy].</param>
        /// <param name="formName">Name of the form.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveFormInstance(int tenantId, int folderVersionId, int formDesignVersionId, int formInstanceId, int accountId, int folderId, int refFolderId, bool isCopy, bool isReference, string formName)
        {
            ServiceResult result = new ServiceResult();
            string referenceSection = string.Empty;
            int newFormInstanceId = 0;

            string isUniqueDocumentName = _settingManager.GetSettingValue("UniqueDocumentName").ToLower();
            if (isUniqueDocumentName == "yes" && folderVersionService.IsFormInstanceExist(formName))
            {
                ServiceResultItem resultItem = new ServiceResultItem();
                resultItem.Messages = new string[1] { "Document name is already exist, please enter unique document name." };
                resultItem.Status = ServiceResultStatus.Failure;
                result.Items = new List<ServiceResultItem>() { resultItem };
                result.Result = ServiceResultStatus.Failure;
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            if (isReference == true)
            {
                formDesignVersionId = Convert.ToInt32(ConfigurationManager.AppSettings["ReferenceFormVersionId"] ?? "0");
                if (formDesignVersionId == 0)
                    return Json(result, JsonRequestBehavior.AllowGet);
            }

            if (tenantId != 0 && folderVersionId != 0 && (formDesignVersionId > 0 || formInstanceId > 0))
            {
                result = this.folderVersionService.CreateFormInstance(tenantId, folderVersionId, formDesignVersionId, formInstanceId, isCopy, formName, CurrentUserName);
                newFormInstanceId = Convert.ToInt32(result.Items.FirstOrDefault().Messages.FirstOrDefault());
                if (isReference == true)
                    SaveReferenceDocument(tenantId, accountId, refFolderId, folderId, folderVersionId, formInstanceId, newFormInstanceId);

                if (result.Result == ServiceResultStatus.Success)
                {
                    FormInstanceViewModel viewModel = this.folderVersionService.GetFormInstance(tenantId, newFormInstanceId);
                    viewModel.IsFormInstanceEditable = true;
                    //Lock newly created/copied document
                    List<int> formInstanceIDs = new List<int>();

                    //Get all views of Document from selected view
                    List<DocumentViewListViewModel> viewList = folderVersionService.GetDocumentViewList(tenantId, viewModel.FormInstanceID);

                    //List of formInstanceIDs of all views
                    formInstanceIDs = viewList.Select(c => c.FormInstanceId).ToList();

                    _resourceLockService.UpdateDocumentLockStatus(viewModel.FolderID, CurrentUserId, formInstanceIDs, base.CurrentUserName);
                    return Json(viewModel, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Gets the form name.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        public JsonResult GetFormNameToCopy(int tenantId, int formInstanceId)
        {
            FormInstanceViewModel formName = this.folderVersionService.GetFormNameToCopy(tenantId, formInstanceId);
            return Json(formName, JsonRequestBehavior.AllowGet);
        }

        #endregion

        /// <summary>
        /// This method passes paras to Preview.cshtml to generate url with given paras
        /// 12/23/2014 SH Modified method: passed roleID as a parameter and updated FormInstanceExportPDF model
        /// </summary>
        /// <param name="formInstanceId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="folderVersionId"></param>
        /// <returns></returns>
        //[AllowAnonymous]
        public ActionResult PreviewFormInstance(int formInstanceId, int formDesignVersionId, int folderVersionId, string formName, string accountName, string folderName, string folderVersionNumber, DateTime effectiveDate, int templateId, int roleID)
        {
            FormInstanceExportPDF model = new FormInstanceExportPDF();
            model.FormInstanceID = formInstanceId;
            model.FormDesignVersionID = formDesignVersionId;
            model.FolderVersionID = folderVersionId;
            model.FormName = formName;
            model.AccountName = accountName;
            model.FolderName = folderName;
            model.FolderVersionNumber = folderVersionNumber;
            model.EffectiveDate = effectiveDate;
            model.TemplateID = templateId;
            model.RoleID = roleID;
            model.GenerationDate = DateTime.Now;
            return View("Preview", model);
        }

        /// <summary>
        /// PrintPDf method generates PDF document of current form with wkhtmltopdf tool using url.
        /// 12/23/2014 SH Modidied method: Retrieved RolID and passed to URL.
        /// </summary>
        /// <returns>PDF file with Download option</returns>
        //[AllowAnonymous]
        public ActionResult PrintPDF(int formInstanceId, int formDesignVersionId, int folderVersionId, string formName, int tenantId, int accountId, string folderName, string folderVersionNumber, DateTime effectiveDate, int templateId)
        {
            string accountName = string.Empty;
            if (accountId == 0)
            {
                accountName = "";
            }
            else
            {
                accountName = _consumerAccountService.GetAccountName(tenantId, accountId);
            }

            string formInstancePdfReportScheme = WebConfigurationManager.AppSettings["FormInstancePdfReportScheme"].ToString();
            string formInstancePdfReportHost = WebConfigurationManager.AppSettings["FormInstancePdfReportHost"].ToString();
            string formInstancePdfReportPort = WebConfigurationManager.AppSettings["FormInstancePdfReportPort"].ToString();
            string formInstancePdfReportUrl = WebConfigurationManager.AppSettings["FormInstancePdfReportUrl"].ToString();
            var formInstanceIdUrl = formInstancePdfReportUrl.Replace("formInstanceId=", "formInstanceId=" + formInstanceId);
            var formDesignVersionIdUrl = formInstanceIdUrl.Replace("formDesignVersionId=", "formDesignVersionId=" + formDesignVersionId);
            var folderVersionIdUrl = formDesignVersionIdUrl.Replace("folderVersionId=", "folderVersionId=" + folderVersionId);
            var formNameUrl = folderVersionIdUrl.Replace("formName=", "formName=" + formName);
            var accountNameUrl = formNameUrl.Replace("accountName=", "accountName=" + accountName);
            var folderNameUrl = accountNameUrl.Replace("folderName=", "folderName=" + folderName);
            var folderVersionNumberUrl = folderNameUrl.Replace("folderVersionNumber=", "folderVersionNumber=" + folderVersionNumber);
            var effectiveDateUrl = folderVersionNumberUrl.Replace("effectiveDate=", "effectiveDate=" + effectiveDate);
            var templateIdUrl = effectiveDateUrl.Replace("templateId=", "templateId=" + templateId);
            var roleIdUrl = templateIdUrl.Replace("roleID=", "roleID=" + RoleID);
            var url = formInstancePdfReportScheme + "://" + formInstancePdfReportHost + ":" + formInstancePdfReportPort + roleIdUrl;

            MemoryStream fileStream = GetPDFStream(url, formName);

            var fileDownloadName = formName + ".pdf";
            var contentType = "application/pdf";

            var fsr = new FileStreamResult(fileStream, contentType);
            fsr.FileDownloadName = fileDownloadName;

            return fsr;
        }

        /// <summary>
        /// Generates the memorystream by converting the bytes from respective url
        /// </summary>
        /// <param name="url"></param>
        /// <returns>MemoryStream required to generate PDF</returns>
        public MemoryStream GetPDFStream(string url, string formName)
        {
            var settings = new HtmlToPdfDocument()
            {
                GlobalSettings =
                {
                    UseCompression = true,
                    ProduceOutline = true,
                    Orientation = TuesPechkin.GlobalSettings.PaperOrientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins =
                    {
                        Top = 1.00,
                        Right = 0.75,
                        Bottom = 1.00,
                        Left = 0.75,
                        Unit = Unit.Centimeters
                    },
                }
            };

            var objectSetting = new ObjectSettings()
            {
                PageUrl = url,
                ProduceForms = false,

                HeaderSettings = new HeaderSettings
                {
                    //LeftText = "[section] : [subsection] : [subsubsection]",
                    RightText = "Page " + "[page]",
                    FontSize = 10,
                    FontName = "Calibri",
                    UseLineSeparator = true,
                },

                LoadSettings =
                {
                    BlockLocalFileAccess = false,
                    StopSlowScript = false,
                    DebugJavascript = false
                },

                WebSettings =
                {
                    //MinimumFontSize = 12,
                    LoadImages = true,
                    PrintBackground = true,
                    PrintMediaType = true,
                    EnableJavascript = true,
                    EnablePlugins = false,
                    EnableIntelligentShrinking = true
                }
            };
            settings.Objects.Add(objectSetting);

            IPechkin pdfDocument = Factory.Create();
            byte[] pdf = pdfDocument.Convert(settings);

            MemoryStream fileStream = new MemoryStream(pdf);

            return fileStream;
        }

        public JsonResult GetAccountList(int tenantId)
        {
            IEnumerable<ConsumerAccountViewModel> accountList = this._consumerAccountService.GetAccountList(tenantId);
            if (accountList == null)
            {
                accountList = new List<ConsumerAccountViewModel>();
            }
            return Json(accountList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the folder list form which the form instances will be copied.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="accountType">Type of the account.</param>
        /// <returns></returns>
        public JsonResult GetFolderListReference(int tenantId, int accountId)
        {
            IEnumerable<FolderVersionViewModel> folderList = null;

            folderList = folderVersionService.GetFolderList(tenantId, accountId);

            if (folderList == null)
                folderList = new List<FolderVersionViewModel>();

            return Json(folderList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the folder list form which the form instances will be copied.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="accountType">Type of the account.</param>
        /// <returns></returns>
        public JsonResult GetFolderVersionList(int tenantId, int folderId)
        {
            IEnumerable<FolderVersionViewModel> folderVersionList = null;

            folderVersionList = folderVersionService.GetFolderVersionList(tenantId, folderId);

            if (folderVersionList == null)
                folderVersionList = new List<FolderVersionViewModel>();

            return Json(folderVersionList.OrderByDescending(row => row.FolderVersionId), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Delete selected Folder on Accout search page
        /// </summary>
        /// <param name="tenantId">TenantID</param>
        /// <param name="folderID">FolderID which are need deleted</param>
        /// <returns>Retuen sucess or failure result</returns>
        public ActionResult DeleteNonPortfolioBasedFolder(int tenantId, int folderID)
        {
            ServiceResult result = new ServiceResult();

            //result = this._facetTranslatorService.IsFolderExistInQueue(tenantId, folderID);
            //if (result.Result == ServiceResultStatus.Success)
            {
                result = this.folderVersionService.DeleteNonPortfolioBasedFolder(tenantId, folderID);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFolderVersion(int folderVersionID)
        {
            FolderVersionViewModel folderVersion = this.folderVersionService.GetFolderVersion(folderVersionID);

            return Json(folderVersion, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMasterListFormDesignID(int folderVersionId)
        {
            int? MLformDesignID;
            MLformDesignID = this.folderVersionService.GetMasterListFormDesignID(folderVersionId);
            return Json(MLformDesignID == null ? 0 : MLformDesignID, JsonRequestBehavior.AllowGet); ;
        }


        public JsonResult GetUserFolderVersionCreationPermission(bool isPortfolioSearch)
        {
            bool IsuserAbletoCreateFolderVersion = this.folderVersionService.GetUserFolderVersiontCreationPermission(base.CurrentUserId, isPortfolioSearch);
            return Json(IsuserAbletoCreateFolderVersion, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStateAccessRoles(int tenantId, int folderVersionId)
        {
            List<WorkFlowVersionStatesAccessViewModel> stateAccessRoles = workflowStateService.GetWorkFlowStateUserRoles(tenantId, folderVersionId);
            return Json(stateAccessRoles, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeMLProcessingOrQueued()
        {
            bool result = false;
            MasterListCascadeBatchViewModel model = _masterListCascadeService.GetQueuedBatch();
            if (model != null)
            {
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion Action Methods

        #region private Methods
        private void InitializeViewBags()
        {
            ViewBag.Claims = ControllerHelper.FilterClaimsByMethod(Claims, GetActionName);
            ViewBag.RoleId = RoleID;
            ViewBag.IsHiddenOrDisableSections = IdentityManager.IsHiddenOrDisableSections.ToString().ToLower();
            ViewBag.IsHiddenContainer = IdentityManager.IsHiddenContainer.ToString().ToLower(); ;
            ViewBag.formInstanceClaims = IdentityManager.GetResourceClaims(identitymanagement.Enums.ResourceType.FORM).Where(c => c.RoleID == RoleID).ToList();
            ViewBag.ApplyFieldMask = "false";
            ViewBag.IsFolderLockEnable = IdentityManager.IsFolderLockEnable.ToString().ToLower();
            ViewBag.IsStopScrollFloatingHeaders = IdentityManager.IsStopScrollFloatingHeaders.ToString().ToLower();
            ViewBag.CurrentUserName = CurrentUserName;
            ViewBag.MasterListEffectivedate = ConfigurationManager.AppSettings["MasterListEffectiveDate"].ToString();
            ViewBag.GridType = System.Configuration.ConfigurationManager.AppSettings["GridType"] ?? "PQ";

            if (Request.Params["fromEV"] != null)
            {
                ViewBag.IsFromEV = true;
            }
            else
            {
                ViewBag.IsFromEV = false;
            }
        }


        private void InitializeEmailSettings()
        {
            sendGridUserName = ConfigurationManager.AppSettings["SendGridUserName"] ?? string.Empty;
            sendGridPassword = ConfigurationManager.AppSettings["SendGridPassword"] ?? string.Empty;
        }

        private void SaveReferenceDocument(int tenantId, int accountId, int refFolderId, int folderId, int folderVersionId, int formInstanceId, int newFormInstanceId)
        {
            int formDesignVersionId = Convert.ToInt32(ConfigurationManager.AppSettings["ReferenceFormVersionId"] ?? "0");
            string formDesignVersion = _formDesignServices.GetCompiledFormDesignVersion(tenantId, formDesignVersionId);
            FormDesignVersionDetail detail = JsonConvert.DeserializeObject<FormDesignVersionDetail>(formDesignVersion);
            JObject jsondata = JObject.Parse(detail.JSONData);

            ReferenceDocumentViewModel refModel = folderVersionService.GetReferenceDocumentModel(tenantId, accountId, refFolderId, folderId, formInstanceId);

            jsondata.SelectToken("Reference.Account").Replace(refModel.AccountName);
            jsondata.SelectToken("Reference.Folder").Replace(refModel.FolderName);
            jsondata.SelectToken("Reference.FolderVersionEffectiveDate").Replace(refModel.FolderVersionEffectiveDate.ToShortDateString());
            jsondata.SelectToken("Reference.FolderVersion").Replace(refModel.FolderVersionNumber);
            jsondata.SelectToken("Reference.Document").Replace(refModel.DocumentName);
            jsondata.SelectToken("Reference.AccountID").Replace(accountId);
            jsondata.SelectToken("Reference.FolderID").Replace(folderId);
            jsondata.SelectToken("Reference.FolderVersionID").Replace(refFolderId);
            jsondata.SelectToken("Reference.FormInstanceID").Replace(formInstanceId);

            string jsonWithValues = JsonConvert.SerializeObject(jsondata);
            ServiceResult resultRef = this.folderVersionService.SaveFormInstanceData(tenantId, folderVersionId, newFormInstanceId, jsonWithValues, base.CurrentUserName);

            //Save Product Reference Mappings
            this.folderVersionService.CreateFormReference(accountId, folderId, refFolderId, formInstanceId, refModel.ConsortiumID, newFormInstanceId, CurrentUserName);
        }

        public static string ToCsvRow(string separator, object o)
        {
            //Type t = typeof(T);
            Type t = o.GetType();
            PropertyInfo[] props = t.GetProperties();
            PropertyInfo[] filterprops = props.Where(p => p.Name != "").ToArray();

            StringBuilder linie = new StringBuilder();

            foreach (var f in filterprops)
            {
                if (linie.Length > 0)
                    linie.Append(separator);

                var x = f.GetValue(o);

                if (x != null)
                {
                    if (f.Name == "GRGR_TERM_DT")
                    {
                        linie.Append((Convert.ToDateTime(x.ToString())).ToShortDateString());
                    }
                    else if (x.ToString() == "")
                    {
                        linie.Append("Data Not Available");
                    }
                    else
                    {
                        linie.Append(x.ToString().Trim());
                    }
                }
                else
                {
                    linie.Append("Data Not Available");
                }
            }
            return linie.ToString();
        }

        public string GetAcceleratedConfirmationMsg(int wfversionstateId)
        {
            string message = workflowStateService.GetAcceleratedConfirmationMsg(wfversionstateId);
            return message;
        }
        #endregion

        [HttpPost]
        public ActionResult UploadProofingDocuments(int formInstanceId) //HttpFileCollectionBase filesToUpload, 
        {
            int result = 0;
            string timeStamp = string.Empty;
            Dictionary<string, string> dictFilePathMappings = new Dictionary<string, string>();

            try
            {
                if (Request.Files.Count > 0)
                {
                    //Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    List<string> FileNamList = new List<string>();
                    string fileName;
                    string fileUploadPath = ConfigurationManager.AppSettings["ProofingDocuments"] + formInstanceId;
                    bool exists = Directory.Exists(fileUploadPath);

                    if (!exists)
                        Directory.CreateDirectory(fileUploadPath); //Server.MapPath(
                    for (int i = 0; i < files.Count; i++)
                    {
                        KeyValue filePathMap = new KeyValue();
                        HttpPostedFileBase file = files[i];
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] fileNames = file.FileName.Split(new char[] { '\\' });
                            fileName = fileNames[fileNames.Length - 1];
                        }
                        else
                            fileName = file.FileName;

                        //int millisecond = DateTime.Now.Millisecond;
                        string dateTimeStamp = DateTime.Now.ToString().Replace('/', '-').Replace(':', '-'); // + "-" + millisecond;
                        string[] SplitStr = fileName.Split('.');
                        string FileUniqueName = SplitStr[0] + "_" + dateTimeStamp + "." + SplitStr[1];
                        string fname = Path.Combine(fileUploadPath, FileUniqueName);
                        file.SaveAs(fname);
                        timeStamp = DateTime.Now.ToString();
                        dictFilePathMappings.Add(fileName, fname);
                    }
                }
                else
                {
                    timeStamp = DateTime.Now.ToString();
                    result = 1;
                }
            }
            catch (Exception ex)
            {

            }
            return Json(new { result, CurrentUserName, timeStamp, dictFilePathMappings });
        }

        public FileResult DownloadProofingDocument(string fileName)
        {
            string fileDownloadName = "Proofing Document";
            if (fileName.Length > 0)
            {
                string[] strArray = fileName.Split(new char[] { '\\' });
                fileDownloadName = strArray[strArray.Length - 1];
            }
            return File(fileName, System.Net.Mime.MediaTypeNames.Application.Octet, fileDownloadName);
        }

        public ActionResult GetTasksFoldersList()
        {

            IQueryable<FolderViewModel> objList;
            try
            {
                objList = folderVersionService.GetFolderList(base.TenantID, base.CurrentUserId);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
            return Json(objList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTasksFolderVersionsList(int folderId)
        {

            IQueryable<FolderVersionViewModel> objList;
            try
            {
                objList = folderVersionService.GetFolderVersionsList(base.TenantID, base.CurrentUserId, folderId);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
            return Json(objList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTasksFormInstanceListForFolderVersion(int folderVersionId, int folderId)
        {

            List<FormInstanceViewModel> objList;
            try
            {
                objList = folderVersionService.GetFormInstanceListForFolderVersion(base.TenantID, folderVersionId, folderId);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
            return Json(objList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPBPDocumentsSharePointLink()
        {
            string strDocumentsLink = string.Empty;
            try
            {
                strDocumentsLink = ConfigurationManager.AppSettings["ProofingDocumentsSPLink"].ToString();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
            return Json(strDocumentsLink, JsonRequestBehavior.AllowGet);
        }

        private List<String> GetLockedFoldersForMLCascade(int folderVersionId)
        {
            List<string> messages = new List<String>();
            FolderVersionViewModel viewmodel = folderVersionService.GetFolderVersionById(folderVersionId);
            //Release FolderVersion which has Status ="In Progress"
            string comment = string.IsNullOrEmpty(viewmodel.Comments) ? "Cascade : Masterlist has been released" : "Cascade : " + viewmodel.Comments;
            MasterListFormDesignViewModel designModel = _masterListService.GetFolderVersionFormDesign(folderVersionId);
            List<MasterListCascadeViewModel> mlCascades = _masterListCascadeService.GetMasterListCascade(designModel.FormDesignID, designModel.FormDesignVersionID);
            //check for Folders open for editing
            List<tmg.equinox.applicationservices.viewmodels.masterListCascade.DocumentFilterResult> results = new List<tmg.equinox.applicationservices.viewmodels.masterListCascade.DocumentFilterResult>();
            foreach (var mlCascade in mlCascades)
            {
                MasterListVersions mlVers = new MasterListVersions();
                mlVers.CurrentFolderVersionID = folderVersionId;
                mlVers.CurrentFormInstanceID = designModel.FormInstanceID;
                tmg.equinox.core.masterlistcascade.filter.DocumentFilter filter = new tmg.equinox.core.masterlistcascade.filter.DocumentFilter(_masterListCascadeService, folderVersionService, null, mlCascade, mlVers, viewmodel.EffectiveDate, 0);
                filter.ProcessFilter(mlCascade, mlVers, 0, ref results);
            }
            //get locked documents
            List<ResourceLock> lockedDocuments = _resourceLockService.GetLockedDocuments();
            var filteredDocs = from cas in results
                               join lck in lockedDocuments on cas.FolderID equals lck.FolderID
                               select new
                               {
                                   FolderName = lck.FolderName,
                                   UserName = lck.LockedUserName
                               };

            if (filteredDocs != null && filteredDocs.Count() > 0)
            {
                List<String> folderNames = new List<String>();
                foreach (var flt in filteredDocs)
                {
                    if (!folderNames.Contains(flt.FolderName))
                    {
                        string msg = "";
                        folderNames.Add(flt.FolderName);
                        msg = "Folder " + flt.FolderName + " is locked by " + flt.UserName + ".";
                        messages.Add(msg);
                    }
                }
            }
            return messages;
        }

        private void SendMessageToUsersOfLockedFoldersForMLCascade(int folderVersionId)
        {
            try
            {
                FolderVersionViewModel viewmodel = folderVersionService.GetFolderVersionById(folderVersionId);
                MasterListFormDesignViewModel designModel = _masterListService.GetFolderVersionFormDesign(folderVersionId);
                List<MasterListCascadeViewModel> mlCascades = _masterListCascadeService.GetMasterListCascade(designModel.FormDesignID, designModel.FormDesignVersionID);
                //check for Folders open for editing
                List<tmg.equinox.applicationservices.viewmodels.masterListCascade.DocumentFilterResult> results = new List<tmg.equinox.applicationservices.viewmodels.masterListCascade.DocumentFilterResult>();
                foreach (var mlCascade in mlCascades)
                {
                    MasterListVersions mlVers = new MasterListVersions();
                    mlVers.CurrentFolderVersionID = folderVersionId;
                    mlVers.CurrentFormInstanceID = designModel.FormInstanceID;
                    tmg.equinox.core.masterlistcascade.filter.DocumentFilter filter = new tmg.equinox.core.masterlistcascade.filter.DocumentFilter(_masterListCascadeService, folderVersionService, null, mlCascade, mlVers, viewmodel.EffectiveDate, 0);
                    filter.ProcessFilter(mlCascade, mlVers, 0, ref results);
                }
                //get locked documents
                List<ResourceLock> lockedDocuments = _resourceLockService.GetLockedDocuments();
                var filteredDocs = from cas in results
                                   join lck in lockedDocuments on cas.FolderID equals lck.FolderID
                                   select new
                                   {
                                       FolderName = lck.FolderName,
                                       UserName = lck.LockedUserName
                                   };

                var distinctFolderLocks = filteredDocs.Distinct().ToList();
                if (distinctFolderLocks != null && distinctFolderLocks.Count() > 0)
                {
                    foreach (var flt in distinctFolderLocks)
                    {
                        List<Paramters> paramater = new List<Paramters>();
                        paramater.Add(new Paramters { key = "message", Value = "Master List Cascade will start in 5 minutes. Please Save and Exit the folder. Please check Cascade Status Screen for completion status" });
                        _notificationService.SendNotification(
                                    new NotificationInfo
                                    {
                                        SentTo = flt.UserName,
                                        MessageKey = MessageKey.TASK_EV_COMPLETED,
                                        ParamterValues = paramater,
                                        loggedInUserName = flt.UserName,
                                    });
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
