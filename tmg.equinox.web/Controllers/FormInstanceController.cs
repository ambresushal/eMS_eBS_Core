using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.applicationservices.viewmodels.ConsumerAccount;
using tmg.equinox.web.FormInstance;
using System.IO;
using tmg.equinox.applicationservices.viewmodels;
using System.Collections;
using tmg.equinox.web.Framework.Caching;
using tmg.equinox.web.Validator;
using tmg.equinox.web.CustomRule;
using Newtonsoft.Json.Linq;
using tmg.equinox.web.Handler;
using System.Dynamic;
using Newtonsoft.Json.Converters;
using tmg.equinox.web.Framework.MasterList;
using tmg.equinox.applicationservices.viewmodels.CollateralWindowService;
using tmg.equinox.web.FormInstanceManager;
using tmg.equinox.web.FormInstanceProcessor;
using tmg.equinox.web.FormDesignManager;
using tmg.equinox.web.FormInstanceProcessor.ExpressionBuilder;
using System.Configuration;
using tmg.equinox.applicationservices;
using tmg.equinox.dependencyresolution;
using tmg.equinox.schema.Base.Model;
using tmg.equinox.repository.interfaces;
using System.Threading.Tasks;
using tmg.equinox.identitymanagement;
//using tmg.equinox.web.export;
using tmg.equinox.rules.oongroups;
using tmg.equinox.infrastructure.util;
using tmg.equinox.web.SOTView;
using tmg.equinox.domain.entities;
using System.Web;
using tmg.equinox.web.FindnReplace;
using tmg.equinox.web.extensions;
using System.Web.Script.Serialization;
using System.Reflection;
using System.Text;
using System.Data;
using tmg.equinox.setting.Interface;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.setting;

namespace tmg.equinox.web.Controllers
{
    public class FormInstanceController : AuthenticatedController
    {
        #region Private Members
        private IFolderVersionServices _folderVersionServices;
        private IFormDesignService _formDesignServices;
        private IDataValueService _dataValueServices;
        private IConsumerAccountService _consumerAccountService;
        private IFormInstanceRepeaterService _formInstanceRepeaterService;
        private IMasterListService _masterListService;
        private IReportingDataService _reportingDataService;
        private const string COSTSHARE_SECTION_NAME = "CostShare";
        private const string NETWORK_SECTION_NAME = "Network";
        private const string BENEFIT_REVIEW = "BenefitReview";
        private IFormInstanceDataServices _formInstanceDataServices;
        private IUIElementService _uiElementService;
        private IFormInstanceService _formInstanceService;
        private IFolderLockService _folderLockService;
        private bool _inMemoryFolderLock = false;
        private IResourceLockService _resourceLockService;
        private IFormInstanceRuleExecutionLogService _ruleExecutionLogService;
        private ISectionLockService _sectionLockService;
        private ISettingManager _settingManager;
        private IExitValidateService _exitValidateService;
        #endregion Private Members

        #region Constructor
        public FormInstanceController(IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, IDataValueService dataValueServices, IConsumerAccountService consumerAccountService, IFormInstanceRepeaterService formInstanceRepeaterService, IMasterListService masterListService, IFormInstanceDataServices formInstanceDataServices, IUIElementService uiElementService, IFormInstanceService formInstanceService, IFolderLockService folderLockService, IReportingDataService reportingDataService, IResourceLockService resourceLockService, IFormInstanceRuleExecutionLogService ruleExecutionLogService, ISectionLockService sectionLockService, ISettingManager settingManager, IExitValidateService exitValidateService)
        {
            this._folderVersionServices = folderVersionServices;
            this._formDesignServices = formDesignServices;
            this._dataValueServices = dataValueServices;
            this._consumerAccountService = consumerAccountService;
            this._formInstanceRepeaterService = formInstanceRepeaterService;
            this._masterListService = masterListService;
            this._formInstanceDataServices = formInstanceDataServices;
            this._uiElementService = uiElementService;
            this._formInstanceService = formInstanceService;
            this._folderLockService = folderLockService;
            this._inMemoryFolderLock = Convert.ToString(ConfigurationManager.AppSettings["FolderLockToUse"]) == "InMemory" ? true : false;
            _reportingDataService = reportingDataService;
            _resourceLockService = resourceLockService;
            _ruleExecutionLogService = ruleExecutionLogService;
            _sectionLockService = sectionLockService;
            _settingManager = settingManager;
            _exitValidateService = exitValidateService;
        }
        #endregion Constructor

        #region Action Methods


        public JsonResult ManageSectionLock(int folderId, int formInstanceId, string displayViewName, string displaySectionName, string sectionName, int formDesignID, string formName, bool isMasterList)
        {
            var result = _sectionLockService.ManageSectionLock(folderId, formInstanceId, displayViewName, displaySectionName, sectionName, base.CurrentUserId.Value, base.CurrentUserName, formDesignID, formName, isMasterList);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ReleaseDocumentAndSectionLockOnTimeOut()
        {
            var result = _resourceLockService.ReleaseDocumentAndSectionLock(base.CurrentUserId.Value);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ReleaseSectionLockonDocumentClose(int formInstanceId, string previousSectionName)
        {
            var result = _sectionLockService.ReleaseSectionLockonDocumentCloseOrSectionChange(formInstanceId, base.CurrentUserId.Value, previousSectionName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //[AllowAnonymous]
        public JsonResult GetFormInstanceDesignData(int tenantId, int formInstanceId, int formDesignVersionId, int folderVersionId, bool reloadData, string sectionName, bool rulesPreloaded, string viewMode = "Default", string ancillaryProductName = "")
        {
            bool isNewLoadedVersionIsMajorVersion = false;
            int effectiveFormDesignVersionId = this._formDesignServices.GetEffectiveFormDesignVersion(base.CurrentUserName, tenantId,
                                                                       formInstanceId, formDesignVersionId, folderVersionId);
            if (effectiveFormDesignVersionId > 0)
            {
                formDesignVersionId = effectiveFormDesignVersionId;
                isNewLoadedVersionIsMajorVersion = this._folderVersionServices.UpdateWithEffectiveFormDesinVersionID(base.CurrentUserName, tenantId,
                                                                                  folderVersionId);
            }

            bool isfolderReleased = !this._folderVersionServices.IsMasterListFolderVersionInProgress(folderVersionId);
            FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(tenantId, formDesignVersionId, _formDesignServices);

            FormDesignVersionDetail detail = formDesignVersionMgr.GetFormDesignVersion(false);

            FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(detail.TenantID, base.CurrentUserId, _formInstanceDataServices, base.CurrentUserName, _folderVersionServices);

            IDataProcessor dataProcessor = new FormInstanceDataProcessor(_folderVersionServices, formInstanceId, folderVersionId, formDesignVersionId, isfolderReleased, CurrentUserId, formDataInstanceManager, _formInstanceDataServices, _uiElementService, detail, base.CurrentUserName, _formDesignServices, _masterListService, _formInstanceService, _ruleExecutionLogService, ancillaryProductName);

            if (sectionName == "")
            {
                sectionName = detail.Sections[0].FullName;
            }

            if (detail.IsMasterList)
            {
                reloadData = true;
            }
            //dataProcessor.RunProcessorOnSectionLoad(sectionName, reloadData);
            dataProcessor.RunProcessorOnDocumentLoad(sectionName, reloadData);

            //empty the custom rules as we have a separate call to load Custom Rules
            detail.CustomRules = "";
            detail.IsNewLoadedVersionIsMajorVersion = isNewLoadedVersionIsMajorVersion;
            if (rulesPreloaded == true)
            {
                detail.Rules = null;
                detail.Validations = null;
            }
            return Json(detail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFormInstanceDesign(int tenantId, int formInstanceId, int formDesignVersionId, int folderVersionId, bool reloadData, bool rulesPreloaded)
        {
            bool isNewLoadedVersionIsMajorVersion = false;
            int effectiveFormDesignVersionId = this._formDesignServices.GetEffectiveFormDesignVersion(base.CurrentUserName, tenantId,
                                                                       formInstanceId, formDesignVersionId, folderVersionId);
            if (effectiveFormDesignVersionId > 0)
            {
                formDesignVersionId = effectiveFormDesignVersionId;
                isNewLoadedVersionIsMajorVersion = this._folderVersionServices.UpdateWithEffectiveFormDesinVersionID(base.CurrentUserName, tenantId,
                                                                                  folderVersionId);
            }
            bool isfolderReleased = !this._folderVersionServices.IsMasterListFolderVersionInProgress(folderVersionId);

            int latestFormDesignVersionId = this._formDesignServices.GetLatestFormDesignVersion(formDesignVersionId);
            FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(tenantId, formDesignVersionId, _formDesignServices);
            FormDesignVersionDetail detail = formDesignVersionMgr.GetFormDesignVersion(false);
            FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(detail.TenantID, base.CurrentUserId, _formInstanceDataServices, base.CurrentUserName, _folderVersionServices);

            IDataProcessor dataProcessor = new FormInstanceCompleteDataProcessor(_folderVersionServices, formInstanceId, folderVersionId, formDesignVersionId, isfolderReleased, CurrentUserId, formDataInstanceManager, _formInstanceDataServices, _uiElementService, detail, base.CurrentUserName, _formDesignServices, _masterListService, _formInstanceService, _ruleExecutionLogService);
            dataProcessor.RunProcessorOnDocumentLoad(string.Empty, reloadData);

            //empty the custom rules as we have a separate call to load Custom Rules
            detail.CustomRules = "";
            detail.IsNewLoadedVersionIsMajorVersion = isNewLoadedVersionIsMajorVersion;
            if (rulesPreloaded == true)
            {
                detail.Rules = null;
                detail.Validations = null;
            }
            if (latestFormDesignVersionId != formDesignVersionId)
            {
                formDesignVersionMgr = new FormDesignVersionManager(tenantId, latestFormDesignVersionId, _formDesignServices);
                FormDesignVersionDetail latestDesign = formDesignVersionMgr.GetFormDesignVersion(false);
                DesignSyncForSOTView designSyn = new DesignSyncForSOTView(detail, latestDesign);

                detail.Sections = designSyn.GetUpdatedTargetDetail().Sections;
                detail.IsNewVersionMerged = true;
            }
            else
            {
                detail.IsNewVersionMerged = false;
            }
            return Json(detail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFormInstanceData(int tenantId, int formInstanceId, int formDesignVersionId, int folderVersionId)
        {
            FormDesignDataCacheHandler formDesignCacheHandler = new FormDesignDataCacheHandler();
            string formDesignData = formDesignCacheHandler.Get(tenantId, formDesignVersionId, _formDesignServices);
            FormDesignVersionDetail detail = JsonConvert.DeserializeObject<FormDesignVersionDetail>(formDesignData);

            IDataCacheHandler cacheHandler = DataCacheHandlerFactory.GetHandler();
            detail.JSONData = cacheHandler.Get(tenantId, formInstanceId, false, detail, _folderVersionServices, CurrentUserId);

            return Json(detail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFormInstanceSectionData(int formInstanceId, string sectionName, int formDesignId, int folderVersionId, int formDesignVersionId)
        {
            List<SectionDetails> sectionDetails = new List<SectionDetails>();

            FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(1, formDesignVersionId, _formDesignServices);
            FormDesignVersionDetail detail = formDesignVersionMgr.GetFormDesignVersion(false);

            FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(detail.TenantID, base.CurrentUserId, _formInstanceDataServices, base.CurrentUserName, _folderVersionServices);

            FormInstanceDataProcessor dataProcessor = new FormInstanceDataProcessor(_folderVersionServices, formInstanceId, folderVersionId, formDesignVersionId, false, CurrentUserId, formDataInstanceManager, _formInstanceDataServices, _uiElementService, detail, base.CurrentUserName, _formDesignServices, _masterListService, _formInstanceService, _ruleExecutionLogService);
            dataProcessor.RunProcessorOnSectionLoad(sectionName, false);

            SectionDesign sectionDesign = detail.Sections.Where(a => a.GeneratedName == sectionName).FirstOrDefault();
            SectionDetails sectionDetail = new SectionDetails();

            DataSourceDesign sectionInlineDataSource = detail.DataSources.Where(a => a.TargetParent.Split('.')[0].ToString() == sectionName && a.DisplayMode == "In Line").FirstOrDefault();

            if (sectionInlineDataSource != null)
            {
                sectionDetail.DataSource = detail.DataSources;
            }
            sectionDetail.SectionDetail = sectionDesign;
            sectionDetail.SectionData = detail.JSONData;

            sectionDetails.Add(sectionDetail);

            return Json(sectionDetails, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDocumentViewList(int tenantId, int formInstanceId)
        {
            List<DocumentViewListViewModel> viewList = this._folderVersionServices.GetDocumentViewList(tenantId, formInstanceId);
            if (viewList == null)
            {
                viewList = new List<DocumentViewListViewModel>();
            }
            foreach (var view in viewList)
            {
                if (view.FormDesignID == 2409)
                {
                    view.FormDesignDisplayName = view.FormInstanceName;
                }
            }
            return Json(viewList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveFormInstanceData(int tenantId, int formInstanceId, int folderId, int folderVersionId, int formDesignId, int formDesignVersionId, string formInstanceData, string repeaterFullNameList, string repeaterData, string sectionName, string commentData)
        {
            ServiceResult result = new ServiceResult();
            result = _formInstanceDataServices.SaveFormInstanceCommentLog(formInstanceId, formDesignId, formDesignVersionId, CurrentUserName, commentData);

            formInstanceData = _formInstanceDataServices.AddNodeToSectionData(sectionName, formInstanceData);
            List<JToken> sectionList = new List<JToken>();

            FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(tenantId, formDesignVersionId, _formDesignServices);
            FormDesignVersionDetail detail = formDesignVersionMgr.GetFormDesignVersion(false);

            FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(tenantId, base.CurrentUserId, _formInstanceDataServices, base.CurrentUserName, _folderVersionServices, _reportingDataService, _masterListService);
            string previousData = formDataInstanceManager.GetSectionData(formInstanceId, sectionName, false, detail, false, false);
            formDataInstanceManager.SetCacheData(formInstanceId, sectionName, formInstanceData);

            IDataProcessor dataProcessor = new FormInstanceDataProcessor(_folderVersionServices, formInstanceId, folderVersionId, formDesignVersionId, false, CurrentUserId, formDataInstanceManager, _formInstanceDataServices, _uiElementService, detail, base.CurrentUserName, _formDesignServices, _masterListService, _formInstanceService, _ruleExecutionLogService);
            dataProcessor.RunProcessorOnDocumentSave(sectionName, false, formInstanceData, previousData);
            try
            {
                result = formDataInstanceManager.SaveSectionsData(formInstanceId, true, _folderVersionServices, _formDesignServices, detail, sectionName);
            }
            catch (Exception ex)
            {
                string appName = config.Config.GetApplicationName();
                if (appName.ToLower() == "emedicaresync")
                {
                    int pbpFormInstanceId = _exitValidateService.GetPBPViewFormInstanceID(folderVersionId, formInstanceId);
                    if (_exitValidateService.IsExitValidateInProgress(pbpFormInstanceId, folderVersionId))
                    {
                        result.Result = ServiceResultStatus.Warning;
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            //List<SettingDefinition> settingModel = _settingManager.GetSettingInfo().ToList();
            //var issettingOn = settingModel.Where(row => row.SettingName == "Reportingcenter_OnOff").FirstOrDefault();
            //if (issettingOn != null)
            //{
            //    if (result == null || result.Result == ServiceResultStatus.Success && issettingOn.DefaultValue.ToString() == "ON")
            //        Task.Run(() => formDataInstanceManager.SaveFormInstanceDataIntoReportingCenterDB(tenantId, CurrentUserId, base.CurrentUserName, formInstanceId, folderId));

            //}

            result = dataProcessor.UpdateAccountProductMap(folderId, folderVersionId, _consumerAccountService, result);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveFormInstance(int tenantId, int formInstanceId, int folderId, int folderVersionId, int formDesignId, int formDesignVersionId, string formInstanceData)
        {
            ServiceResult result = null;

            FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(tenantId, formDesignVersionId, _formDesignServices);
            FormDesignVersionDetail detail = formDesignVersionMgr.GetFormDesignVersion(false);

            FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(tenantId, base.CurrentUserId, _formInstanceDataServices, base.CurrentUserName, _folderVersionServices, _reportingDataService, _masterListService);
            IDataProcessor dataProcessor = new FormInstanceCompleteDataProcessor(_folderVersionServices, formInstanceId, folderVersionId, formDesignVersionId, false, CurrentUserId, formDataInstanceManager, _formInstanceDataServices, _uiElementService, detail, base.CurrentUserName, _formDesignServices, _masterListService, _formInstanceService, _ruleExecutionLogService);

            IDataCacheHandler cacheHandler = DataCacheHandlerFactory.GetHandler();
            var prevFormData = cacheHandler.Get(tenantId, formInstanceId, false, detail, _folderVersionServices, CurrentUserId);
            cacheHandler.Add(tenantId, formInstanceId, CurrentUserId, formInstanceData);

            dataProcessor.RunProcessorOnDocumentSave(string.Empty, false, formInstanceData, prevFormData);

            result = cacheHandler.Save(_folderVersionServices, tenantId, folderVersionId, formInstanceId, formInstanceData, CurrentUserName, CurrentUserId, _reportingDataService, formDesignId, formDesignVersionId);
            formDataInstanceManager.SaveTargetSectionsData(formInstanceId, _folderVersionServices, _formDesignServices);
            //if (result.Result == ServiceResultStatus.Success)
            //    Task.Run(() => formDataInstanceManager.SaveFormInstanceDataIntoReportingCenterDB(tenantId, CurrentUserId, base.CurrentUserName, formInstanceId, folderId));
            result = dataProcessor.UpdateAccountProductMap(folderId, folderVersionId, _consumerAccountService, result);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFormInstanceCommentData(int formInstanceId)
        {
            var data = this._formInstanceDataServices.GetFormInstanceCommentLog(formInstanceId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ValidateInput(false)]
        public JsonResult UpdateFormInstanceSectionData(string newSection, int tenantId, int formInstanceId, int folderVersionId, int formDesignId, int formDesignVersionId, string sectionName, string sectionData)
        {
            ServiceResult result = null;
            CustomRuleResult returnResult = new CustomRuleResult();
            returnResult.updatedSections = new List<SectionResult>();

            sectionData = _formInstanceDataServices.AddNodeToSectionData(sectionName, sectionData);

            FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(tenantId, formDesignVersionId, _formDesignServices);
            FormDesignVersionDetail detail = formDesignVersionMgr.GetFormDesignVersion(false);

            FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(tenantId, CurrentUserId, _formInstanceDataServices, CurrentUserName, _folderVersionServices);

            string previousData = formDataInstanceManager.GetSectionData(formInstanceId, sectionName, false, detail, false, false);
            formDataInstanceManager.SetCacheData(formInstanceId, sectionName, sectionData);

            SectionSaveHashCheckOptimizer hashOptimizer = new SectionSaveHashCheckOptimizer(previousData, sectionData);
            bool isDataChange = hashOptimizer.hasSectionChanged();
            bool isSectionLockEnabled = false;
            isSectionLockEnabled = _sectionLockService.IsSectionLevelLockingEnabledForFormDesign(formDesignId);
            if (isDataChange)
            {
                FormInstanceDataProcessor dataProcessor = new FormInstanceDataProcessor(_folderVersionServices, formInstanceId, folderVersionId, formDesignVersionId, false, CurrentUserId, formDataInstanceManager, _formInstanceDataServices, _uiElementService, detail, base.CurrentUserName, _formDesignServices, _masterListService, _formInstanceService, _ruleExecutionLogService);
                dataProcessor.RunProcessorOnSectionSave(sectionName, false, sectionData, previousData);

                //AutoSave data before releasing section
                if (isSectionLockEnabled)
                {
                    result = formDataInstanceManager.SaveSectionsData(formInstanceId, true, _folderVersionServices, _formDesignServices, detail, sectionName);
                }
            }
            if (isSectionLockEnabled)
            {
                ReleaseSectionLockonDocumentClose(formInstanceId, sectionName);
            }
            returnResult.updatedSections.Add(new SectionResult(sectionName, sectionData));
            returnResult.updatedSections = returnResult.updatedSections.Where(s => s.SectionName != sectionName).ToList();
            return Json(returnResult.updatedSections, JsonRequestBehavior.AllowGet);
        }

        public JavaScriptResult LoadCustomRules(int tenantId, int formInstanceId, int formDesignVersionId, int folderVersionId)
        {
            FormDesignVersionDetail detail = FormDesignVersionDetailReadOnlyObjectCache.GetFormDesignVersionDetail(tenantId, formDesignVersionId, _formDesignServices);
            string customRules = detail != null ? detail.CustomRules : "";
            return JavaScript(customRules);
        }
        [ValidateInput(false)]
        public ActionResult ExportToExcel(string csv, bool isGroupHeader, int noOfColInGroup, bool isChildGrid,
            string repeaterName, string formName, int folderVersionId, int folderId, int tenantId)
        {
            string header = string.Empty;
            csv = HttpUtility.UrlDecode(csv);
            //FolderVersionViewModel folderVersionViewModel = _folderVersionServices.GetFolderVersion(CurrentUserName,tenantId, folderVersionId, folderId);
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

        [ValidateInput(false)]
        public ActionResult ExportToExcelActivityLog(string csv, bool isGroupHeader, int noOfColInGroup, bool isChildGrid,
            string repeaterName, string formName, int folderVersionId, int folderId, int tenantId, int formInstnaceId)
        {
            DataTable activityLogDataList = this._folderVersionServices.GetActivityLogData(formInstnaceId);
            string header = string.Empty;

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
            MemoryStream fileStream = excelBuilder.DownloadPreviewGridExcel(activityLogDataList, isGroupHeader, noOfColInGroup, isChildGrid, header, repeaterName);
            var fileDownloadName = header.Trim() + ".xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            var fsr = new FileStreamResult(fileStream, contentType);
            fsr.FileDownloadName = fileDownloadName;

            return fsr;
        }

        public ActionResult GetSourceFormInstanceData(int tenantId, int targetFormInstanceId, int formDesignVersionId, int folderVersionId, int formDesignId, string fullName)
        {
            int sourceFormInstanceID = this._folderVersionServices.GetSourceFormInstanceID(targetFormInstanceId, formDesignVersionId, folderVersionId, formDesignId);
            FormInstanceParser formInstanceParser = new FormInstanceParser();
            string data = "";
            if (targetFormInstanceId == sourceFormInstanceID)
            {
                string sectionName = fullName.Split('.').First();
                IDataCacheHandler cacheHandler = DataCacheHandlerFactory.GetHandler();
                data = cacheHandler.GetSection(targetFormInstanceId, sectionName, CurrentUserId);
            }
            else
            {
                data = this._folderVersionServices.GetFormInstanceData(tenantId, sourceFormInstanceID);
            }
            var sourceRepeaterData = formInstanceParser.GetValue(data, fullName);
            var sourceData = JsonConvert.SerializeObject(sourceRepeaterData);
            return Json(sourceData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteFormInstance(int folderID, int tenantId, int formInstanceId, int folderVersionId)
        {
            ServiceResult result = this._folderVersionServices.DeleteFormInstance(folderID, tenantId, folderVersionId, formInstanceId, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult IsDataSource(int formDesignID, int formDesignVersionID)
        {
            bool isDataSource = this._folderVersionServices.IsDataSource(formDesignID, formDesignVersionID);
            return Json(isDataSource, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertiesData(int folderVersionId)
        {
            var propertiesData = this._folderVersionServices.GetFolderVersionProperties(folderVersionId);
            return Json(propertiesData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCopyAuditTrailData(int folderVersionId)
        {
            var audittrailData = this._folderVersionServices.GetCopyFromAuditTrailData(folderVersionId);
            return Json(audittrailData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult HasFolderLockByCurrentUser(int folderId)
        {
            var result = _inMemoryFolderLock ? _folderLockService.IsFolderLocked(folderId, CurrentUserId) : this._folderVersionServices.HasFolderLockByCurrentUser(CurrentUserId, folderId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsDocumentLockedByCurrentUser(int formInstanceId, string sectionName, int formDesignId)
        {
            var result = _inMemoryFolderLock ? _resourceLockService.IsDocumentLocked(formInstanceId, CurrentUserId, sectionName, formDesignId) : this._folderVersionServices.HasFolderLockByCurrentUser(CurrentUserId, formInstanceId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFormInstanceRepeaterData(int formInstanceId, string fullName, GridPagingRequest gridPagingRequest)
        {
            GridPagingResponse<object> repeaterList = this._formInstanceRepeaterService.GetFormInstanceRepeaterData(formInstanceId, fullName, gridPagingRequest);

            return Json(repeaterList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is used to retrieve form design data with form instance data. used explicitely for PDF generation piece of code.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formInstanceId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="folderVersionId"></param>
        /// <returns></returns>
        public JsonResult GetFormInstanceRepeaterDesignData(int tenantId, int formInstanceId, int formDesignVersionId, int folderVersionId)
        {

            bool isNewLoadedVersionIsMajorVersion = false;
            int effectiveFormDesignVersionId = this._formDesignServices.GetEffectiveFormDesignVersion(base.CurrentUserName, tenantId,
                                                                       formInstanceId, formDesignVersionId, folderVersionId);
            if (effectiveFormDesignVersionId > 0)
            {
                formDesignVersionId = effectiveFormDesignVersionId;
                isNewLoadedVersionIsMajorVersion = this._folderVersionServices.UpdateWithEffectiveFormDesinVersionID(base.CurrentUserName, tenantId,
                                                                                  folderVersionId);
            }

            bool isfolderReleased = !this._folderVersionServices.IsMasterListFolderVersionInProgress(folderVersionId);

            FormInstanceDataHandler formDataHandler = new FormInstanceDataHandler(_folderVersionServices, _formDesignServices, formInstanceId, folderVersionId, formDesignVersionId, isfolderReleased, CurrentUserId, _masterListService);
            FormDesignVersionDetail detail = formDataHandler.ExecuteDocumentHandler(true);

            if (formDataHandler.IsNewFormInstance)
            {
                IDataCacheHandler cacheHandler = DataCacheHandlerFactory.GetHandler();
                cacheHandler.Save(_folderVersionServices, tenantId, folderVersionId, formInstanceId, detail.JSONData, User.Identity.Name, CurrentUserId, _reportingDataService, 0, formDesignVersionId);
            }

            detail.JSONData = _folderVersionServices.applDesignRule(detail);

            //empty the custom rules as we have a separate call to load Custom Rules
            detail.CustomRules = "";
            detail.IsNewLoadedVersionIsMajorVersion = isNewLoadedVersionIsMajorVersion;
            return Json(detail, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckDuplication(int formInstanceId, string fullname, List<string> existingRowIndexList, string rowData, List<string> duplicationObject)
        {
            Boolean result = this._formInstanceRepeaterService.CheckDuplication(formInstanceId, fullname, rowData, existingRowIndexList, duplicationObject);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCheckDuplicationList(int tenantId, int formInstanceId, int formDesignVersionId, string repeaterFullNameList)
        {
            IDictionary<string, object> duplicationList = this._formInstanceRepeaterService.GetDuplicatedElementsList(repeaterFullNameList, formInstanceId, formDesignVersionId);

            return Json(duplicationList, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public JsonResult ValidateFormInstance(int tenantId, int formInstanceId, int folderVersionId, int folderId, string sectionName, string sectionData, bool isValidateAllDocs, int formDesignVersionId, bool validateOnlyCurrentSection)
        {
            //Update section in cache
            IDataCacheHandler cacheHandler = DataCacheHandlerFactory.GetHandler();
            cacheHandler.UpdateSection(formInstanceId, sectionName, sectionData, CurrentUserId);
            FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(tenantId, base.CurrentUserId, _formInstanceDataServices, base.CurrentUserName, _folderVersionServices);
            //Update all visited sections in cache
            try
            {
                FormInstanceSectionDataCacheHandler _handler = new FormInstanceSectionDataCacheHandler();
                List<JToken> sections = _handler.GetSectionListFromCache(formInstanceId, CurrentUserId);
                foreach (var item in sections)
                {
                    var secName = item.ToString();
                    if (!string.Equals(secName, sectionName))
                    {
                        JObject secData = JObject.Parse(_handler.IsExists(Convert.ToInt32(formInstanceId), secName, CurrentUserId));
                        cacheHandler.UpdateSection(formInstanceId, secName, secData[secName].ToString(), CurrentUserId);
                    }
                }
            }
            catch (Exception ex) { }
            MasterListReader ms = new MasterListReader(tenantId, _masterListService);
            JObject masterListData = ms.GetCompleteData(folderVersionId);
            bool isCBCMasterList = true;

            //Get list of all forms in a folder if folder validation is requested else validate single form
            List<FormInstanceViewModel> formInstancesAll = new List<FormInstanceViewModel>();
            List<FormInstanceViewModel> formInstances = new List<FormInstanceViewModel>();
            if (isValidateAllDocs)
            {
                //formInstances = this._folderVersionServices.GetFormInstanceList(tenantId, folderVersionId, folderId, 1);
                formInstancesAll = this._folderVersionServices.GetFormInstanceList(tenantId, folderVersionId, folderId, 0);
                //formInstances = this._folderVersionServices.GetProductList(tenantId, folderVersionId);
                //FormDesigns to validate

                formInstances = formInstancesAll.Where(row => row.AnchorDocumentID == row.FormInstanceID).ToList();

                List<int> formInstanceIDs = new List<int>();
                foreach (var form in formInstances)
                {
                    try
                    {
                        PBPExportPreProcessor preprocess = new PBPExportPreProcessor(CurrentUserId, CurrentUserName, _formDesignServices, _folderVersionServices, _formInstanceDataServices, _uiElementService, _formInstanceService, _reportingDataService, _masterListService, form.FormInstanceID);
                        preprocess.ProcessRulesAndSaveSectionsValidation();

                        FormInstanceSectionDataCacheHandler _handler = new FormInstanceSectionDataCacheHandler();
                        List<JToken> sections = _handler.GetSectionListFromCache(formInstanceId, CurrentUserId);
                        foreach (var item in sections)
                        {
                            var secName = item.ToString();
                            if (!string.Equals(secName, sectionName))
                            {
                                JObject secData = JObject.Parse(_handler.IsExists(Convert.ToInt32(formInstanceId), secName, CurrentUserId));
                                cacheHandler.UpdateSection(formInstanceId, secName, secData[secName].ToString(), CurrentUserId);
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    var isformDataInCache = cacheHandler.IsExists(tenantId, form.FormInstanceID, CurrentUserId);
                    if (isformDataInCache == null)
                        formInstanceIDs.Add(form.FormInstanceID);
                }

                List<FormInstanceViewModel> formInstancesdata = this._folderVersionServices.GetMultipleFormInstancesData(tenantId, formInstanceIDs);
                foreach (var instance in formInstancesdata)
                {
                    FormInstanceViewModel viewModel = formInstances.Where(t => t.FormInstanceID == instance.FormInstanceID).FirstOrDefault();
                    if (viewModel != null)
                        viewModel.FormData = instance.FormData;
                }

                cacheHandler.AddMultiple(tenantId, _folderVersionServices, _formDesignServices, formInstancesdata, CurrentUserId);
            }
            else
            {
                formInstances.Add(this._folderVersionServices.GetFormInstance(tenantId, formInstanceId));
            }
            FolderVersionViewModel model = this._folderVersionServices.GetFolderVersion(CurrentUserId, base.CurrentUserName, tenantId, folderVersionId, folderId);
            int? masterListFormDesignID = this._folderVersionServices.GetMasterListFormDesignID(folderVersionId);
            if (masterListFormDesignID == CustomRuleConstants.MasterListFormDesignID)
                isCBCMasterList = true;
            else
                isCBCMasterList = false;

            List<dynamic> validationDataList = null;
            DocumentValidatorManager validateMgr = new DocumentValidatorManager(formInstances, model, masterListData, formDataInstanceManager, _folderVersionServices, _formDesignServices, _formInstanceService);
            if (validateOnlyCurrentSection)
            {
                validationDataList = validateMgr.ExecuteValidation(tenantId, _formDesignServices, _folderVersionServices, CurrentUserId, isCBCMasterList, model.IsPortfolio, sectionName);
            }
            else
            {
                try
                {
                    PBPExportPreProcessor preprocess = new PBPExportPreProcessor(CurrentUserId, CurrentUserName, _formDesignServices, _folderVersionServices, _formInstanceDataServices, _uiElementService, _formInstanceService, _reportingDataService, _masterListService, formInstanceId);
                    preprocess.ProcessRulesAndSaveSectionsValidation();

                    FormInstanceSectionDataCacheHandler _handler = new FormInstanceSectionDataCacheHandler();
                    List<JToken> sections = _handler.GetSectionListFromCache(formInstanceId, CurrentUserId);
                    foreach (var item in sections)
                    {
                        var secName = item.ToString();
                        if (!string.Equals(secName, sectionName))
                        {
                            JObject secData = JObject.Parse(_handler.IsExists(Convert.ToInt32(formInstanceId), secName, CurrentUserId));
                            cacheHandler.UpdateSection(formInstanceId, secName, secData[secName].ToString(), CurrentUserId);
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                validationDataList = validateMgr.ExecuteValidation(tenantId, _formDesignServices, _folderVersionServices, CurrentUserId, isCBCMasterList, model.IsPortfolio, null);
            }

            return Json(validationDataList, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public JsonResult ValidateFormInstanceData(int tenantId, int formInstanceId, int folderVersionId, int folderId, string formData, bool isValidateAllDocs, int formDesignVersionId, bool validateOnlyCurrentSection, string sectionName)
        {
            //Update section in cache
            IDataCacheHandler cacheHandler = DataCacheHandlerFactory.GetHandler();
            cacheHandler.Add(tenantId, formInstanceId, CurrentUserId, formData);

            MasterListReader ms = new MasterListReader(tenantId, _masterListService);
            JObject masterListData = ms.GetCompleteData(folderVersionId);
            bool isCBCMasterList = true;

            //Get list of all forms in a folder if folder validation is requested else validate single form
            List<FormInstanceViewModel> formInstances = new List<FormInstanceViewModel>();
            if (isValidateAllDocs)
            {
                formInstances = this._folderVersionServices.GetFormInstanceList(tenantId, folderVersionId, folderId, 1);
                //formInstances = this._folderVersionServices.GetProductList(tenantId, folderVersionId);

                List<int> formInstanceIDs = new List<int>();
                foreach (var form in formInstances)
                {
                    var isformDataInCache = cacheHandler.IsExists(tenantId, form.FormInstanceID, CurrentUserId);
                    if (isformDataInCache == null)
                        formInstanceIDs.Add(form.FormInstanceID);
                }

                List<FormInstanceViewModel> formInstancesdata = this._folderVersionServices.GetMultipleFormInstancesData(tenantId, formInstanceIDs);
                foreach (var instance in formInstancesdata)
                {
                    FormInstanceViewModel viewModel = formInstances.Where(t => t.FormInstanceID == instance.FormInstanceID).FirstOrDefault();
                    if (viewModel != null)
                        viewModel.FormData = instance.FormData;
                }

                cacheHandler.AddMultiple(tenantId, _folderVersionServices, _formDesignServices, formInstancesdata, CurrentUserId);
            }
            else
            {
                formInstances.Add(this._folderVersionServices.GetFormInstance(tenantId, formInstanceId));
            }
            FolderVersionViewModel model = this._folderVersionServices.GetFolderVersion(CurrentUserId, base.CurrentUserName, tenantId, folderVersionId, folderId);
            int? masterListFormDesignID = this._folderVersionServices.GetMasterListFormDesignID(folderVersionId);
            if (masterListFormDesignID == CustomRuleConstants.MasterListFormDesignID)
                isCBCMasterList = true;
            else
                isCBCMasterList = false;

            List<dynamic> validationDataList = null;
            FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(tenantId, base.CurrentUserId, _formInstanceDataServices, base.CurrentUserName, _folderVersionServices);
            DocumentValidatorManager validateMgr = new DocumentValidatorManager(formInstances, model, masterListData, true, formInstanceId, formDataInstanceManager, _folderVersionServices, _formDesignServices, _formInstanceService);
            if (validateOnlyCurrentSection && !string.IsNullOrEmpty(sectionName))
            {
                validationDataList = validateMgr.ExecuteValidation(tenantId, _formDesignServices, _folderVersionServices, CurrentUserId, isCBCMasterList, model.IsPortfolio, sectionName);
            }
            else
            {
                validationDataList = validateMgr.ExecuteValidation(tenantId, _formDesignServices, _folderVersionServices, CurrentUserId, isCBCMasterList, model.IsPortfolio, null);
            }

            return Json(validationDataList, JsonRequestBehavior.AllowGet);
        }

        //[ValidateInput(false)]
        //public JsonResult ValidateExitValidationForFolderversion(int folderVersionId)
        //{
        //    ServiceResult result = new ServiceResult();
        //    var isExitValidationCompleted = _exitValidateService.CheckExitValidationCompletedForFolderVersion(folderVersionId);
        //    if (isExitValidationCompleted)
        //        result.Result = ServiceResultStatus.Success;
        //    else
        //    {
        //        result.Result = ServiceResultStatus.Failure;
        //        _exitValidateService.ExitValidateFolderversion(folderVersionId, (Int32)CurrentUserId, CurrentUserName);
        //        //-----------------------

        //    }
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetActivityLogData(int formInstnaceId, GridPagingRequest gridPagingRequest)
        {
            GridPagingResponse<ActivityLogModel> activityLogDataList = this._folderVersionServices.GetActivityLogData(formInstnaceId, gridPagingRequest);

            return Json(activityLogDataList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveFormInstanceActivityLogData(int tenantId, int formInstanceId, int folderId, int folderVersionId, int formDesignId, int formDesignVersionId, string activityLogFormInstanceData)
        {
            ServiceResult result = null;
            List<ActivityLogModel> repeaterDataJsonObject = JsonConvert.DeserializeObject<List<ActivityLogModel>>(activityLogFormInstanceData);
            result = this._folderVersionServices.SaveFormInstanceAvtivitylogData(tenantId, formInstanceId, folderId, folderVersionId, formDesignId, formDesignVersionId, repeaterDataJsonObject);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAnnotations(int formInstanceId, int formDesignVersionId)
        {
            FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(1, formDesignVersionId, _formDesignServices);
            FormDesignVersionDetail detail = formDesignVersionMgr.GetFormDesignVersion(false);

            AnnotationProcessor processor = new AnnotationProcessor(formInstanceId, _formDesignServices, _formInstanceDataServices, _folderVersionServices, CurrentUserId, CurrentUserName, detail);
            var result = processor.Process();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public FileResult AnnotationReport(string tenantId, string source)
        {
            //get source row
            string jsonStringSource = HttpUtility.UrlDecode(source);
            List<AnnotationViewModel> annotations = JsonConvert.DeserializeObject<List<AnnotationViewModel>>(jsonStringSource);

            AnnotationExcelGenerator generator = new AnnotationExcelGenerator();
            byte[] reportBytes = generator.GenerateExcelReport(annotations);

            string fileName = "Annotations";
            var fileDownloadName = fileName + ".xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(reportBytes, contentType, fileDownloadName);
        }

        public JsonResult UpdateFormInstanceSOTData(int tenantId, int formInstanceId, int folderId, int folderVersionId, int formDesignVersionId, string formInstanceData)
        {
            //ServiceResult result = null;

            FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(tenantId, formDesignVersionId, _formDesignServices);
            FormDesignVersionDetail detail = formDesignVersionMgr.GetFormDesignVersion(false);

            IDataCacheHandler cacheHandler = DataCacheHandlerFactory.GetHandler();
            var prevFormData = cacheHandler.Get(tenantId, formInstanceId, false, detail, _folderVersionServices, CurrentUserId);
            cacheHandler.Add(tenantId, formInstanceId, CurrentUserId, formInstanceData);

            string sectionData = "";
            FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(tenantId, CurrentUserId, _formInstanceDataServices, CurrentUserName, _folderVersionServices);
            foreach (SectionDesign section in detail.Sections)
            {
                JObject fullJson = JObject.Parse(formInstanceData);
                JToken sectionAtoken = fullJson.SelectToken(section.FullName);
                JObject dataJObj = JObject.Parse("{'" + section.FullName + "':[]}");
                dataJObj[section.FullName] = sectionAtoken;
                sectionData = JsonConvert.SerializeObject(dataJObj);
                formDataInstanceManager.SetCacheData(formInstanceId, section.FullName, sectionData);

            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        //CUSTOM: for OON Groups
        public JsonResult ProcessOONGroups(int tenantId, int anchorFormInstanceId, int formInstanceId)
        {
            //get details
            FormInstanceViewModel modelAnchor = _folderVersionServices.GetFormInstance(tenantId, anchorFormInstanceId);
            FormInstanceViewModel modelView = _folderVersionServices.GetFormInstance(tenantId, formInstanceId);

            //process save rules
            FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(tenantId, modelAnchor.FormDesignVersionID, _formDesignServices);
            FormDesignVersionDetail sourceDetail = formDesignVersionMgr.GetFormDesignVersion(false);
            FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(tenantId, base.CurrentUserId, _formInstanceDataServices, base.CurrentUserName, _folderVersionServices, _reportingDataService, _masterListService);
            //IDataProcessor dataProcessor = new FormInstanceDataProcessor(_folderVersionServices, formInstanceId, modelAnchor.FolderVersionID, modelAnchor.FormDesignVersionID, false, CurrentUserId, formDataInstanceManager, _formInstanceDataServices, _uiElementService, detail, base.CurrentUserName, _formDesignServices, _masterListService, _formInstanceService);
            FormDesignVersionDetail targetDetail = FormDesignVersionDetailReadOnlyObjectCache.GetFormDesignVersionDetail(tenantId, modelView.FormDesignVersionID, _formDesignServices);
            string targetSectionOONGroups = formDataInstanceManager.GetSectionData(modelView.FormInstanceID, "OONGroups", false, targetDetail, false, false);
            string targetSectionOONGroupNumbers = formDataInstanceManager.GetSectionData(modelView.FormInstanceID, "OONNumberofGroups", false, targetDetail, false, false);
            IDataCacheHandler handler = DataCacheHandlerFactory.GetHandler();
            string sotData = handler.Get(tenantId, anchorFormInstanceId, false, sourceDetail, _folderVersionServices, CurrentUserId);
            JObject formInstanceData = JObject.Parse(sotData);
            List<JToken> sections = formDataInstanceManager.GetSectionsFromCache(anchorFormInstanceId, CurrentUserId.Value);
            foreach (JToken section in sections)
            {
                string sectionName = section.ToString();
                JObject sectionData = JObject.Parse(formDataInstanceManager.GetSectionData(anchorFormInstanceId, sectionName, false, sourceDetail, false, false));
                formInstanceData[sectionName] = sectionData.SelectToken(sectionName);
            }
            sotData = JsonConvert.SerializeObject(formInstanceData);
            JToken groups = JToken.Parse(targetSectionOONGroups);
            var overrideToken = groups.SelectToken("OONGroups.ManualOverride");
            var prop = overrideToken.Parent as JProperty;
            prop.Value = "False";
            targetSectionOONGroups = groups.ToString();
            OONGroupGenerator groupGen = new OONGroupGenerator(sotData, targetSectionOONGroups, targetSectionOONGroupNumbers, _formInstanceService, modelAnchor.FormDesignVersionID);
            string oonGroupSectionData = groupGen.GetOONGroups();
            string oonGroupNumberSectionData = groupGen.GetOONGroupNumbers();
            FormInstanceSectionDataCacheHandler fiHandler = new FormInstanceSectionDataCacheHandler();
            fiHandler.AddTargetFormInstanceIdToCache(modelAnchor.FormInstanceID, modelView.FormInstanceID, CurrentUserId);
            fiHandler.AddSectionData(modelView.FormInstanceID, "OONGroups", oonGroupSectionData, CurrentUserId);
            fiHandler.AddSectionData(modelView.FormInstanceID, "OONNumberofGroups", oonGroupNumberSectionData, CurrentUserId);
            //save form instance
            ServiceResult result = formDataInstanceManager.SaveSectionsData(modelView.FormInstanceID, true, _folderVersionServices, _formDesignServices, targetDetail, "");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ShowAllProducts(int formInstanceId, int formDesignVersionId, string formName)
        {
            List<FormInstanceAllProductsViewModel> result = new List<FormInstanceAllProductsViewModel>();
            FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(1, base.CurrentUserId, _formInstanceDataServices, base.CurrentUserName, _folderVersionServices, _reportingDataService, _masterListService);
            FormDesignVersionDetail targetDetail = FormDesignVersionDetailReadOnlyObjectCache.GetFormDesignVersionDetail(1, formDesignVersionId, _formDesignServices);
            List<FormInstanceAllProductsViewModel> productsList = new List<FormInstanceAllProductsViewModel>();
            //Product Rules
            string targetProductRulesSection = formDataInstanceManager.GetSectionData(formInstanceId, "ProductRules", false, targetDetail, false, false);
            if (!string.IsNullOrEmpty(targetProductRulesSection))
            {
                JToken productRulesgroups = JToken.Parse(targetProductRulesSection);
                var selectedProductID = productRulesgroups.SelectToken("ProductRules.PlanInformation.ProductID").ToString();
                var selectedproductPlanName = productRulesgroups.SelectToken("ProductRules.PlanInformation.PlanName").ToString();
                var selectedproductProductType = productRulesgroups.SelectToken("ProductRules.PlanInformation.ProductType").ToString();
                if ((!String.IsNullOrEmpty(selectedProductID) && selectedProductID != "NULL") || (!String.IsNullOrEmpty(selectedproductPlanName) && selectedproductPlanName != "NULL") || (!String.IsNullOrEmpty(selectedproductProductType) && selectedproductProductType != "NULL"))
                {
                    productsList.Add(new FormInstanceAllProductsViewModel() { ProductName = selectedproductPlanName, ProductID = selectedProductID, ProductType = selectedproductProductType });
                }
            }
            //Rx product
            string targetRxSection = formDataInstanceManager.GetSectionData(formInstanceId, "Rx", false, targetDetail, false, false);
            if (!string.IsNullOrEmpty(targetRxSection))
            {
                JToken groups = JToken.Parse(targetRxSection);
                var rxSelectedproduct = groups.SelectToken("Rx.RxProductSelection.SelectRxProduct").ToString();
                var rxSelectedProductID = groups.SelectToken("Rx.RxProductInformation.ProductID").ToString();
                var rxSelectedproductPlanName = groups.SelectToken("Rx.RxProductInformation.PlanName").ToString();
                var rxSelectedproductProductType = groups.SelectToken("Rx.RxProductInformation.ProductType").ToString();
                if ((!String.IsNullOrEmpty(rxSelectedProductID) && rxSelectedProductID != "NULL") || (!String.IsNullOrEmpty(rxSelectedproductPlanName) && rxSelectedproductPlanName != "NULL") || (!String.IsNullOrEmpty(rxSelectedproductProductType) && rxSelectedproductProductType != "NULL"))
                {
                    productsList.Add(new FormInstanceAllProductsViewModel() { ProductName = rxSelectedproductPlanName, ProductID = rxSelectedProductID, ProductType = rxSelectedproductProductType });
                }
            }
            //Dental product 
            string targetDentalSection = formDataInstanceManager.GetSectionData(formInstanceId, "Dental", false, targetDetail, false, false);
            if (!string.IsNullOrEmpty(targetDentalSection))
            {
                JToken dentalgroups = JToken.Parse(targetDentalSection);
                var dentalSelectedproduct = dentalgroups.SelectToken("Dental.DentalProductSelection.SelectDentalProduct").ToString();
                var dentalSelectedProductID = dentalgroups.SelectToken("Dental.DentalProductInformation.ProductID").ToString();
                var dentalSelectedproductPlanName = dentalgroups.SelectToken("Dental.DentalProductInformation.PlanName").ToString();
                var dentalSelectedproductProductType = dentalgroups.SelectToken("Dental.DentalProductInformation.ProductType").ToString();
                if ((!String.IsNullOrEmpty(dentalSelectedProductID) && dentalSelectedProductID != "NULL") || (!String.IsNullOrEmpty(dentalSelectedproductPlanName) && dentalSelectedproductPlanName != "NULL") || (!String.IsNullOrEmpty(dentalSelectedproductProductType) && dentalSelectedproductProductType != "NULL"))
                {
                    productsList.Add(new FormInstanceAllProductsViewModel() { ProductName = dentalSelectedproductPlanName, ProductID = dentalSelectedProductID, ProductType = dentalSelectedproductProductType });
                }
            }
            //Vision Product
            string targetVisionSection = formDataInstanceManager.GetSectionData(formInstanceId, "Vision", false, targetDetail, false, false);
            if (!string.IsNullOrEmpty(targetVisionSection))
            {
                JToken visiongroups = JToken.Parse(targetVisionSection);
                var visionSelectedproduct = visiongroups.SelectToken("Vision.VisionProductSelection.SelectVisionProduct").ToString();
                var visionSelectedProductID = visiongroups.SelectToken("Vision.VisionProductInformation.ProductID").ToString();
                var visionSelectedproductPlanName = visiongroups.SelectToken("Vision.VisionProductInformation.PlanName").ToString();
                var visionSelectedproductProductType = visiongroups.SelectToken("Vision.VisionProductInformation.ProductType").ToString();
                if ((!String.IsNullOrEmpty(visionSelectedProductID) && visionSelectedProductID != "NULL") || (!String.IsNullOrEmpty(visionSelectedproductPlanName) && visionSelectedproductPlanName != "NULL") || (!String.IsNullOrEmpty(visionSelectedproductProductType) && visionSelectedproductProductType != "NULL"))
                {
                    productsList.Add(new FormInstanceAllProductsViewModel() { ProductName = visionSelectedproductPlanName, ProductID = visionSelectedProductID, ProductType = visionSelectedproductProductType });
                }
            }
            //Hearing product
            string targetHearingSection = formDataInstanceManager.GetSectionData(formInstanceId, "Hearing", false, targetDetail, false, false);
            if (!string.IsNullOrEmpty(targetHearingSection))
            {
                JToken hearinggroups = JToken.Parse(targetHearingSection);
                var hearingSelectedproduct = hearinggroups.SelectToken("Hearing.HearingProductSelection.SelectHearingProduct").ToString();
                var hearingSelectedProductID = hearinggroups.SelectToken("Hearing.HearingProductInformation.ProductID").ToString();
                var hearingSelectedproductPlanName = hearinggroups.SelectToken("Hearing.HearingProductInformation.PlanName").ToString();
                var hearingSelectedproductProductType = hearinggroups.SelectToken("Hearing.HearingProductInformation.ProductType").ToString();
                if ((!String.IsNullOrEmpty(hearingSelectedProductID) && hearingSelectedProductID != "NULL") || (!String.IsNullOrEmpty(hearingSelectedproductPlanName) && hearingSelectedproductPlanName != "NULL") || (!String.IsNullOrEmpty(hearingSelectedproductProductType) && hearingSelectedproductProductType != "NULL"))
                {
                    productsList.Add(new FormInstanceAllProductsViewModel() { ProductName = hearingSelectedproductPlanName, ProductID = hearingSelectedProductID, ProductType = hearingSelectedproductProductType });
                }
            }

            //HSA product
            string targetHSASection = formDataInstanceManager.GetSectionData(formInstanceId, "HSA", false, targetDetail, false, false);
            if (!string.IsNullOrEmpty(targetHSASection))
            {
                JToken hsagroups = JToken.Parse(targetHSASection);
                //var hsaSelectedproduct = hsagroups.SelectToken("HSA.HSAProductInformation.SelectHearingProduct").ToString();
                var hsaSelectedProductID = hsagroups.SelectToken("HSA.HSAProductInformation.ProductID").ToString();
                var hsaSelectedproductPlanName = hsagroups.SelectToken("HSA.HSAProductInformation.PlanName").ToString();
                var hsaSelectedproductProductType = hsagroups.SelectToken("HSA.HSAProductInformation.ProductType").ToString();
                if ((!String.IsNullOrEmpty(hsaSelectedProductID) && hsaSelectedProductID != "NULL") || (!String.IsNullOrEmpty(hsaSelectedproductPlanName) && hsaSelectedproductPlanName != "NULL") || (!String.IsNullOrEmpty(hsaSelectedproductProductType) && hsaSelectedproductProductType != "NULL"))
                {
                    productsList.Add(new FormInstanceAllProductsViewModel() { ProductName = hsaSelectedproductPlanName, ProductID = hsaSelectedProductID, ProductType = hsaSelectedproductProductType });
                }
            }
            return Json(productsList, JsonRequestBehavior.AllowGet);
        }
        #endregion Action Methods
    }

}