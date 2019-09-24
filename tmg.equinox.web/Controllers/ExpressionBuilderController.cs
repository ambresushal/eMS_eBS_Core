using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using tmg.equinox.anocchart.RuleProcessor;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.applicationservices.viewmodels.masterListCascade;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.ruleinterpreter.rulecompiler;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.web.CustomRule;
using tmg.equinox.web.ExpresionBuilder;
using tmg.equinox.web.FormDesignManager;
using tmg.equinox.web.FormInstanceManager;
using tmg.equinox.web.FormInstanceProcessor;
using tmg.equinox.web.FormInstanceProcessor.ExpressionBuilder;
using tmg.equinox.web.sourcehandler;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.web.FindnReplace;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.anocchart.GlobalUtilities;
using System.IO;
using tmg.equinox.applicationservices.viewmodels.RulesManager;

namespace tmg.equinox.web.Controllers
{
    public class ExpressionBuilderController : AuthenticatedController
    {
        #region Private Members

        private IFolderVersionServices _folderVersionServices;
        private IFormDesignService _formDesignServices;
        private IFormInstanceDataServices _formInstanceDataServices;
        private IMasterListService _masterListService;
        private IUIElementService _uiElementService;
        private IFormInstanceService _formInstanceService;
        private IMasterListCascadeService _mlcService;
        private IRulesManagerService _rulesManagerService;
        #endregion Private Members

        #region Constructor
        public ExpressionBuilderController(IFolderVersionServices folderVersionServices, IFormDesignService formDesignServices, IFormInstanceDataServices formInstanceDataServices, IUIElementService uiElementService, IMasterListService masterListService, IFormInstanceService formInstanceService, IMasterListCascadeService mlcService, IRulesManagerService rulesManagerService)
        {
            this._folderVersionServices = folderVersionServices;
            this._formDesignServices = formDesignServices;
            this._formInstanceDataServices = formInstanceDataServices;
            this._masterListService = masterListService;
            this._uiElementService = uiElementService;
            this._formInstanceService = formInstanceService;
            this._mlcService = mlcService;
            this._rulesManagerService = rulesManagerService;
        }
        #endregion Constructor

        public JsonResult GetLookInList(int formDesignVersionId)
        {
            var uielement = _uiElementService.GetRepeaterElementForLookIn(formDesignVersionId);
            return Json(uielement, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInstances(int folderVersionID)
        {
            var instances = _uiElementService.GetInstances(folderVersionID);
            return Json(instances, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ReplaceText(int folderId, int folderVersionId, int formInstanceId, int formDesignVersionId, string sectionName, string sectionData, string findWhat, string replaceWith, string withIn, string lookIn, bool match, string selectedInstances)
        {
            ServiceResult result = new ServiceResult();

            FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(1, formDesignVersionId, _formDesignServices);
            FormDesignVersionDetail detail = formDesignVersionMgr.GetFormDesignVersion(false);

            DocumentInfo documentInfo = new DocumentInfo() { CurrentSection = sectionName, FolderId = folderId, FolderVersionId = folderVersionId, FormDesignVersionId = formDesignVersionId, FormInstanceId = formInstanceId };
            ReplaceCriteria criteria = new ReplaceCriteria() { FindText = findWhat, IsMatch = match, ReplaceWith = replaceWith, ReplaceWithIn = withIn, SpecificPath = lookIn, SelectedInstances = selectedInstances };
            IReplaceTextProcessor handler = ReplaceTextProcessorFactory.GetHandler(documentInfo, criteria, _formDesignServices, _formInstanceDataServices, _folderVersionServices, CurrentUserId, CurrentUserName, detail);
            var activityLog = handler.Process();

            ActivityLogManager activityMgr = new ActivityLogManager(folderId, folderVersionId, formDesignVersionId, formInstanceId, detail, CurrentUserName, _folderVersionServices, _uiElementService);
            Task.Run(() => activityMgr.SaveActivityLog(activityLog));

            result.Result = ServiceResultStatus.Success;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        // To execute expression builder custom datasource 
        public JsonResult RunCustomDataSources(int tenantId, int folderVersionId, int formInstanceId, int formDesignVersionId, int targetElementId)
        {
            JToken sourceData = null;

            FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(tenantId, formDesignVersionId, _formDesignServices);
            FormDesignVersionDetail details = formDesignVersionMgr.GetFormDesignVersion(true);

            FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(tenantId, base.CurrentUserId, _formInstanceDataServices, base.CurrentUserName, _folderVersionServices);

            {
                CompiledDocumentRule compiledDocumentRule = _uiElementService.GetCompiledRuleJSONForDSExpRule(targetElementId, formDesignVersionId);
                if (null != compiledDocumentRule)
                {
                    SourceHandlerDBManager dbManager = new SourceHandlerDBManager(tenantId, _folderVersionServices, formDataInstanceManager, _formDesignServices, _masterListService);
                    CurrentRequestContext requestContext = new CurrentRequestContext();
                    RuleExecution ruleExecution = new RuleExecution(base.CurrentUserId, compiledDocumentRule, formInstanceId, folderVersionId, dbManager, details, _formDesignServices, _formInstanceService, requestContext);
                    sourceData = ruleExecution.ProcessRule();
                }
            }
            return Json(JsonConvert.SerializeObject(sourceData), JsonRequestBehavior.AllowGet);
        }
        public JsonResult RunChildPopup(int tenantId, int folderVersionId, int formInstanceId, int formDesignVersionId, int targetElementId, string targetElementPath, string targetValue, string uiElementType, string uielementName)
        {
            if(!string.IsNullOrEmpty(uielementName))
            {
                int lastindex = uielementName.LastIndexOf(formInstanceId.ToString());
                uielementName = uielementName.Substring(0, lastindex);                 
                targetElementId = _uiElementService.GetUIElementIDByNames(formDesignVersionId, uielementName);
            }
            JToken sourceData = null;
            FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(tenantId, formDesignVersionId, _formDesignServices);
            FormDesignVersionDetail details = formDesignVersionMgr.GetFormDesignVersion(true);
            FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(tenantId, base.CurrentUserId, _formInstanceDataServices, base.CurrentUserName, _folderVersionServices);
            {
                CompiledDocumentRule compiledDocumentRule = _uiElementService.GetCompiledRuleJSON(targetElementId, formDesignVersionId, true);
                if (null != compiledDocumentRule)
                {
                    SourceHandlerDBManager dbManager = new SourceHandlerDBManager(tenantId, _folderVersionServices, formDataInstanceManager, _formDesignServices, _masterListService);
                    CurrentRequestContext requestContext = new CurrentRequestContext();
                    RuleExecution ruleExecution = new RuleExecution(base.CurrentUserId, compiledDocumentRule, formInstanceId, folderVersionId, dbManager, details, _formDesignServices, _formInstanceService, requestContext);
                    if (uiElementType == "select-one")
                    {
                        sourceData = ruleExecution.ProcessRule(targetElementPath, targetValue);
                    }
                    else
                    {
                        sourceData = ruleExecution.ProcessRule();
                    }
                }
            }
            return Json(JsonConvert.SerializeObject(sourceData), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProcessRuleForManualOverride(int folderVersionId, int formInstanceId, int formDesignVersionId, int targetElementId, string targetElementPath, string sourceElementPath)
        {
            JToken sourceData = null;
            FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(1, formDesignVersionId, _formDesignServices);
            FormDesignVersionDetail details = formDesignVersionMgr.GetFormDesignVersion(true);
            FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(1, base.CurrentUserId, _formInstanceDataServices, base.CurrentUserName, _folderVersionServices);
            {
                string[] source = sourceElementPath.Split('.');
                string sectionData = formDataInstanceManager.GetSectionData(formInstanceId, source[0], false, details, false, false);
                if (!string.IsNullOrEmpty(sectionData))
                {
                    JObject objSectionData = JObject.Parse(sectionData);
                    objSectionData.SelectToken(sourceElementPath).Replace(false);
                    formDataInstanceManager.SetCacheData(formInstanceId, source[0], Convert.ToString(objSectionData));

                    CompiledDocumentRule compiledDocumentRule = _uiElementService.GetCompiledRuleJSON(targetElementId, formDesignVersionId, true);
                    if (null != compiledDocumentRule)
                    {
                        SourceHandlerDBManager dbManager = new SourceHandlerDBManager(1, _folderVersionServices, formDataInstanceManager, _formDesignServices, _masterListService);
                        CurrentRequestContext requestContext = new CurrentRequestContext();
                        RuleExecution ruleExecution = new RuleExecution(base.CurrentUserId, compiledDocumentRule, formInstanceId, folderVersionId, dbManager, details, _formDesignServices, _formInstanceService, requestContext);
                        sourceData = ruleExecution.ProcessRule();
                    }
                }
            }
            return Json(JsonConvert.SerializeObject(sourceData), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProcessRuleForElement(ElementDocumentRuleInputViewModel model)
        {
            int tenantId = 1;
            ElementDocumentRuleViewModelResult resultModel = new ElementDocumentRuleViewModelResult();
            List<ElementDocumentRuleViewModel> ruleModels = _mlcService.GetElementDocumentRules(model.FormDesignVersionID, model.ElementJSONPath);
            int userId = 1;
            string userName = "superuser";
            foreach (ElementDocumentRuleViewModel ruleModel in ruleModels)
            {
                userId = 1;
                int forminstanceid = model.FormInstanceID;
                if (ruleModel.ParentFormDesignVersionID != 0)
                {
                    forminstanceid = _mlcService.getFormInstancePBPView(forminstanceid, ruleModel.FormDesignVersionID);
                    model.ElementJSONPath = ruleModel.TargetFieldPaths.Split(',')[0];

                }

                FormInstanceDataManager formInstanceDataManager = new FormInstanceDataManager(tenantId, base.CurrentUserId, _formInstanceDataServices, "superuser", _folderVersionServices);
                SourceHandlerDBManager sourceDBManager = new SourceHandlerDBManager(tenantId, _folderVersionServices, formInstanceDataManager, _formDesignServices, _masterListService);
                FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(tenantId, ruleModel.FormDesignVersionID, _formDesignServices);
                FormDesignVersionDetail detail = formDesignVersionMgr.GetFormDesignVersion(true);
                CurrentRequestContext requestContext = new CurrentRequestContext();

                string sectionName = model.ElementJSONPath.Split('.')[0];
                string previousData = formInstanceDataManager.GetSectionData(forminstanceid, sectionName, false, detail, false, false);
                JObject previousDataObj = JObject.Parse(previousData);
                JToken token = previousDataObj.SelectToken(model.ElementJSONPath);
                var prop = token.Parent as JProperty;
                prop.Value = model.ElementValue;
                formInstanceDataManager.SetCacheData(forminstanceid, sectionName, JsonConvert.SerializeObject(previousDataObj));
                ExpressionRuleTreeProcessor processor = new ExpressionRuleTreeProcessor(forminstanceid, _uiElementService, model.FolderVersionID,
                sourceDBManager, _formDesignServices, formInstanceDataManager, detail, _formInstanceService, base.CurrentUserId, requestContext);
                var getJson = ruleModel.RuleJSON;
                Documentrule documentRule = DocumentRuleSerializer.Deserialize(getJson);
                DocumentRuleCompiler ruleCompiler = new DocumentRuleCompiler(0, documentRule);
                CompiledDocumentRule compiledRule = ruleCompiler.CompileRule();
                processor.ProcessCompiledRule(compiledRule);

                if (ruleModel.ParentFormDesignVersionID == 0)
                {
                    string resultData = formInstanceDataManager.GetSectionData(forminstanceid, sectionName, false, detail, false, false);
                    resultModel.JSONData = resultData;
                    resultModel.TargetFieldPaths = ruleModel.TargetFieldPaths;
                }


            }
            return Json(JsonConvert.SerializeObject(resultModel), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProcessRuleForTester(int tenantId, int ruleID, int formInstanceId, int folderVersionId, int formDesignVersionId)
        {
            FormInstanceDataManager formInstanceDataManager = new FormInstanceDataManager(tenantId, 1, _formInstanceDataServices, "superuser", _folderVersionServices);
            SourceHandlerDBManager sourceDBManager = new SourceHandlerDBManager(tenantId, _folderVersionServices, formInstanceDataManager, _formDesignServices, _masterListService);
            FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(tenantId, formDesignVersionId, _formDesignServices);
            FormDesignVersionDetail detail = formDesignVersionMgr.GetFormDesignVersion(true);

            CurrentRequestContext requestContext = new CurrentRequestContext();
            ExpressionRuleTreeProcessor processor = new ExpressionRuleTreeProcessor(formInstanceId, _uiElementService, folderVersionId,
            sourceDBManager, _formDesignServices, formInstanceDataManager, detail, _formInstanceService, 1, requestContext);
            string result = null;
            try
            {
                JToken resultData = processor.ProcessRuleTest(ruleID);
                if (resultData != null && resultData.ToString() != "")
                    result = resultData.ToString();
                else
                    result = "Rule did not return any value.";
            }
            catch (System.Exception ex)
            {
                result = ex.Message;
            }
            return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFormInstanceDataCompressed(int tenantId, int formInstanceId)
        {
            JObject data = null;
            string formData = _folderVersionServices.GetFormInstanceDataCompressed(1, formInstanceId);
            if (!String.IsNullOrEmpty(formData))
            {
                data = JObject.Parse(formData);
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetANOCChartServices(int tenantId, int nextYearFormInstanceId, int previousYearFormInstanceId, int anocViewFormInstanceId, DateTime effectiveDate)
        {
            string anocChartServices = string.Empty; string nextYearMedicareJSONData = string.Empty; string prevYearMedicareJSONData = string.Empty;
            string nextYearPBPViewJSONData = string.Empty; string prevYearPBPViewJSONData = string.Empty; string masterListAnocTemplateJSONData = string.Empty;
            string masterListANOCEOCJSONData = string.Empty;
            int viewFormInstanceID = 0;
            try
            {
                if (nextYearFormInstanceId > 0 && previousYearFormInstanceId > 0)
                {
                    int ANOCChartMLId = this._folderVersionServices.GetFormDesignIDByFormName("ANOCChartML");
                    int masterAnocChartFID = this._folderVersionServices.GetMasterListFormInstanceId(nextYearFormInstanceId, ANOCChartMLId, 0);
                    if (masterAnocChartFID > 0)
                        masterListAnocTemplateJSONData = this._folderVersionServices.GetFormInstanceDataCompressed(tenantId, masterAnocChartFID);

                    //List<int> ids = _masterListService.GetFormInstanceIds(effectiveDate, "MedicareANOC/EOC", "HMO-MAPD-CSNP-ISNP");
                    //int masterANOCEOCChartFID = 0;
                    //if (ids != null && ids.Count > 0)
                    //    masterANOCEOCChartFID = ids[0]; //this._folderVersionServices.GetMasterListFormInstanceId(nextYearFormInstanceId, 2387, 0);

                    //if (masterANOCEOCChartFID > 0)
                    //    masterListANOCEOCJSONData = this._folderVersionServices.GetFormInstanceDataCompressed(tenantId, masterANOCEOCChartFID);

                    nextYearMedicareJSONData = this._folderVersionServices.GetFormInstanceDataCompressed(tenantId, nextYearFormInstanceId);
                    prevYearMedicareJSONData = this._folderVersionServices.GetFormInstanceDataCompressed(tenantId, previousYearFormInstanceId);

                    // Added below code for test BRG repeater comparision
                    string anocViewJSONData = this._folderVersionServices.GetFormInstanceDataCompressed(tenantId, anocViewFormInstanceId);


                    // Get PBPView Data for Next Year
                    List<DocumentViewListViewModel> documentViewList = this._folderVersionServices.GetDocumentViewList(tenantId, nextYearFormInstanceId);
                    if (documentViewList != null && documentViewList.Count > 0)
                    {
                        if (documentViewList.Exists(x => x.FormDesignID == 2367))
                        {
                            viewFormInstanceID = documentViewList.Find(x => x.FormDesignID == 2367).FormInstanceId;
                            if (viewFormInstanceID > 0)
                                nextYearPBPViewJSONData = this._folderVersionServices.GetFormInstanceDataCompressed(tenantId, viewFormInstanceID);
                        }
                    }
                    // Get PBPView Data for previous Year
                    documentViewList = this._folderVersionServices.GetDocumentViewList(tenantId, previousYearFormInstanceId);
                    if (documentViewList != null && documentViewList.Count > 0)
                    {
                        if (documentViewList.Exists(x => x.FormDesignID == 2367))
                        {
                            viewFormInstanceID = documentViewList.Find(x => x.FormDesignID == 2367).FormInstanceId;
                            if (viewFormInstanceID > 0)
                                prevYearPBPViewJSONData = this._folderVersionServices.GetFormInstanceDataCompressed(tenantId, viewFormInstanceID);
                        }
                    }

                    string formInstacnename = String.Empty;
                    JToken prevYearData = JObject.Parse(prevYearPBPViewJSONData);
                    string pbpViewPlanType = Convert.ToString(prevYearData.SelectToken(RuleConstants.PlanType) ?? String.Empty);
                    formInstacnename = (pbpViewPlanType == "4" || pbpViewPlanType == "31") ? "PPO-MAPD-ISNP-CSNP" : "HMO-MAPD-CSNP-ISNP";

                    List<int> ids = _masterListService.GetFormInstanceIds(effectiveDate, "MedicareANOC/EOC", formInstacnename);
                    int masterANOCEOCChartFID = 0;
                    if (ids != null && ids.Count > 0)
                        masterANOCEOCChartFID = ids[0]; //this._folderVersionServices.GetMasterListFormInstanceId(nextYearFormInstanceId, 2387, 0);

                    if (masterANOCEOCChartFID > 0)
                        masterListANOCEOCJSONData = this._folderVersionServices.GetFormInstanceDataCompressed(tenantId, masterANOCEOCChartFID);


                    ANOCChartRules chartRuleProcessor = new ANOCChartRules(prevYearMedicareJSONData, nextYearMedicareJSONData, prevYearPBPViewJSONData, nextYearPBPViewJSONData, masterListAnocTemplateJSONData, anocViewJSONData, masterListANOCEOCJSONData);
                    if (!String.IsNullOrEmpty(nextYearMedicareJSONData) && !String.IsNullOrEmpty(prevYearMedicareJSONData) && !String.IsNullOrEmpty(prevYearPBPViewJSONData)
                     && !String.IsNullOrEmpty(nextYearPBPViewJSONData) && !String.IsNullOrEmpty(masterListAnocTemplateJSONData) && !String.IsNullOrEmpty(masterListANOCEOCJSONData))
                    {
                        anocChartServices = chartRuleProcessor.GetANOCChartServices();
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return Json(anocChartServices, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetAdditionalANOCChartServices(int tenantId, int nextYearFormInstanceId, int previousYearFormInstanceId, int formDesignVersionId, int folderVersionId, string sectionName, string repeaterData, DateTime effectiveDate, int anocViewFormInstanceId)
        {
            string anocChartServices = string.Empty; string nextYearMedicareJSONData = string.Empty; string prevYearMedicareJSONData = string.Empty;
            string nextYearPBPViewJSONData = string.Empty; string prevYearPBPViewJSONData = string.Empty; string masterListAnocTemplateJSONData = string.Empty;
            string masterListANOCEOCJSONData = string.Empty;
            int viewFormInstanceID = 0;
            if (nextYearFormInstanceId > 0 && previousYearFormInstanceId > 0)
            {
                int ANOCChartMLId = this._folderVersionServices.GetFormDesignIDByFormName("ANOCChartML");
                int masterAnocChartFID = this._folderVersionServices.GetMasterListFormInstanceId(nextYearFormInstanceId, ANOCChartMLId, 0);
                if (masterAnocChartFID > 0)
                    masterListAnocTemplateJSONData = this._folderVersionServices.GetFormInstanceDataCompressed(tenantId, masterAnocChartFID);

                List<int> ids = _masterListService.GetFormInstanceIds(effectiveDate, "MedicareANOC/EOC", "HMO-MAPD-CSNP-ISNP");
                int masterANOCEOCChartFID = 0;
                if (ids != null && ids.Count > 0)
                    masterANOCEOCChartFID = ids[0]; //this._folderVersionServices.GetMasterListFormInstanceId(nextYearFormInstanceId, 2387, 0);

                if (masterANOCEOCChartFID > 0)
                    masterListANOCEOCJSONData = this._folderVersionServices.GetFormInstanceDataCompressed(tenantId, masterANOCEOCChartFID);

                nextYearMedicareJSONData = this._folderVersionServices.GetFormInstanceDataCompressed(tenantId, nextYearFormInstanceId);
                prevYearMedicareJSONData = this._folderVersionServices.GetFormInstanceDataCompressed(tenantId, previousYearFormInstanceId);

                // Added below code for test BRG repeater comparision
                string anocViewJSONData = this._folderVersionServices.GetFormInstanceDataCompressed(tenantId, anocViewFormInstanceId);


                // Get PBPView Data for Next Year
                List<DocumentViewListViewModel> documentViewList = this._folderVersionServices.GetDocumentViewList(tenantId, nextYearFormInstanceId);
                if (documentViewList != null && documentViewList.Count > 0)
                {
                    if (documentViewList.Exists(x => x.FormDesignID == 2367))
                    {
                        viewFormInstanceID = documentViewList.Find(x => x.FormDesignID == 2367).FormInstanceId;
                        if (viewFormInstanceID > 0)
                            nextYearPBPViewJSONData = this._folderVersionServices.GetFormInstanceDataCompressed(tenantId, viewFormInstanceID);
                    }
                }
                // Get PBPView Data for previous Year
                documentViewList = this._folderVersionServices.GetDocumentViewList(tenantId, previousYearFormInstanceId);
                if (documentViewList != null && documentViewList.Count > 0)
                {
                    if (documentViewList.Exists(x => x.FormDesignID == 2367))
                    {
                        viewFormInstanceID = documentViewList.Find(x => x.FormDesignID == 2367).FormInstanceId;
                        if (viewFormInstanceID > 0)
                            prevYearPBPViewJSONData = this._folderVersionServices.GetFormInstanceDataCompressed(tenantId, viewFormInstanceID);
                    }
                }
                ANOCChartRules chartRuleProcessor = new ANOCChartRules(prevYearMedicareJSONData, nextYearMedicareJSONData, prevYearPBPViewJSONData, nextYearPBPViewJSONData, masterListAnocTemplateJSONData, anocViewJSONData, masterListANOCEOCJSONData);
                if (!String.IsNullOrEmpty(nextYearMedicareJSONData) && !String.IsNullOrEmpty(prevYearMedicareJSONData) && !String.IsNullOrEmpty(prevYearPBPViewJSONData)
                 && !String.IsNullOrEmpty(nextYearPBPViewJSONData) && !String.IsNullOrEmpty(masterListAnocTemplateJSONData) && !String.IsNullOrEmpty(masterListANOCEOCJSONData))
                {
                    anocChartServices = chartRuleProcessor.GetAdditionalANOCChartServices(repeaterData);
                }
            }
            return Json(anocChartServices, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetAdditionalServicesData(int tenantId, int nextYearFormInstanceId, int previousYearFormInstanceId, string repeaterData, string repeaterSelectedData, DateTime effectiveDate, int anocViewFormInstanceId)
        {
            string anocChartServices = string.Empty; string nextYearMedicareJSONData = string.Empty; string prevYearMedicareJSONData = string.Empty;
            string nextYearPBPViewJSONData = string.Empty; string prevYearPBPViewJSONData = string.Empty; string masterListAnocTemplateJSONData = string.Empty;
            string masterListANOCEOCJSONData = string.Empty;
            int viewFormInstanceID = 0;

            if (!String.IsNullOrEmpty(repeaterData) && nextYearFormInstanceId > 0 && previousYearFormInstanceId > 0)
            {
                int ANOCChartMLId = this._folderVersionServices.GetFormDesignIDByFormName("ANOCChartML");
                int masterAnocChartFID = this._folderVersionServices.GetMasterListFormInstanceId(nextYearFormInstanceId, ANOCChartMLId, 0);
                if (masterAnocChartFID > 0)
                    masterListAnocTemplateJSONData = this._folderVersionServices.GetFormInstanceDataCompressed(tenantId, masterAnocChartFID);

                List<int> ids = _masterListService.GetFormInstanceIds(effectiveDate, "MedicareANOC/EOC", "HMO-MAPD-CSNP-ISNP");
                int masterANOCEOCChartFID = 0;
                if (ids != null && ids.Count > 0)
                    masterANOCEOCChartFID = ids[0]; //this._folderVersionServices.GetMasterListFormInstanceId(nextYearFormInstanceId, 2387, 0);

                if (masterANOCEOCChartFID > 0)
                    masterListANOCEOCJSONData = this._folderVersionServices.GetFormInstanceDataCompressed(tenantId, masterANOCEOCChartFID);

                nextYearMedicareJSONData = this._folderVersionServices.GetFormInstanceDataCompressed(tenantId, nextYearFormInstanceId);
                prevYearMedicareJSONData = this._folderVersionServices.GetFormInstanceDataCompressed(tenantId, previousYearFormInstanceId);

                string anocViewJSONData = this._folderVersionServices.GetFormInstanceDataCompressed(tenantId, anocViewFormInstanceId);

                // Get PBPView Data for Next Year
                List<DocumentViewListViewModel> documentViewList = this._folderVersionServices.GetDocumentViewList(tenantId, nextYearFormInstanceId);
                if (documentViewList != null && documentViewList.Count > 0)
                {
                    if (documentViewList.Exists(x => x.FormDesignID == 2367))
                    {
                        viewFormInstanceID = documentViewList.Find(x => x.FormDesignID == 2367).FormInstanceId;
                        if (viewFormInstanceID > 0)
                            nextYearPBPViewJSONData = this._folderVersionServices.GetFormInstanceDataCompressed(tenantId, viewFormInstanceID);
                    }
                }
                // Get PBPView Data for previous Year
                documentViewList = this._folderVersionServices.GetDocumentViewList(tenantId, previousYearFormInstanceId);
                if (documentViewList != null && documentViewList.Count > 0)
                {
                    if (documentViewList.Exists(x => x.FormDesignID == 2367))
                    {
                        viewFormInstanceID = documentViewList.Find(x => x.FormDesignID == 2367).FormInstanceId;
                        if (viewFormInstanceID > 0)
                            prevYearPBPViewJSONData = this._folderVersionServices.GetFormInstanceDataCompressed(tenantId, viewFormInstanceID);
                    }
                }
                ANOCChartRules chartRuleProcessor = new ANOCChartRules(prevYearMedicareJSONData, nextYearMedicareJSONData, prevYearPBPViewJSONData, nextYearPBPViewJSONData, masterListAnocTemplateJSONData, anocViewJSONData, masterListANOCEOCJSONData);
                if (!String.IsNullOrEmpty(nextYearMedicareJSONData) && !String.IsNullOrEmpty(prevYearMedicareJSONData) && !String.IsNullOrEmpty(prevYearPBPViewJSONData)
                 && !String.IsNullOrEmpty(nextYearPBPViewJSONData) && !String.IsNullOrEmpty(masterListAnocTemplateJSONData) && !String.IsNullOrEmpty(masterListANOCEOCJSONData))
                {
                    anocChartServices = chartRuleProcessor.GetAdditionalServicesData(repeaterData, repeaterSelectedData);
                }
            }
            return Json(anocChartServices, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ProcessRuleForSameSection(int folderVersionId, int formInstanceId, int formDesignVersionId, string sourceElementPath, string[] ElementValue, bool isMultiselect)
        {
            List<ExpressionBuilderViewModel> sourceData = new List<ExpressionBuilderViewModel>();
            FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(1, formDesignVersionId, _formDesignServices);
            FormDesignVersionDetail details = formDesignVersionMgr.GetFormDesignVersion(true);
            FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(1, base.CurrentUserId, _formInstanceDataServices, base.CurrentUserName, _folderVersionServices);
            {
                string[] source = sourceElementPath.Split('.');
                string sectionData = formDataInstanceManager.GetSectionData(formInstanceId, source[0], false, details, false, false);
                if (!string.IsNullOrEmpty(sectionData))
                {
                    JObject objSectionData = JObject.Parse(sectionData);
                    try
                    {
                       //JToken ElementValueToken = isMultiselect ? JToken.FromObject(ElementValue) : JToken.Parse(ElementValue[0]);
                       JToken ElementValueToken = null;
                        if (ElementValue != null && ElementValue.Length > 0)
                        {
                            if (isMultiselect)
                            {
                                ElementValueToken = JToken.FromObject(ElementValue);
                            }
                            else if (ElementValue[0].ToString().Contains("["))
                            {
                                ElementValueToken = JToken.Parse(ElementValue[0]);
                            }
                            else
                            {
                                ElementValueToken = JToken.FromObject(ElementValue[0]);
                            }
                        } 
                        objSectionData.SelectToken(sourceElementPath).Replace(ElementValueToken);
                    }
                    catch (Exception ex)
                    {
                        JToken ElementValueTokenEx = JToken.FromObject(ElementValue);
                        objSectionData.SelectToken(sourceElementPath).Replace(ElementValueTokenEx);
                    }

                    formDataInstanceManager.SetCacheData(formInstanceId, source[0], Convert.ToString(objSectionData));
                    List<ruleinterpreter.model.CompiledDocumentRule> compiledDocumentRule = _uiElementService.GetAllDocumentRuleBySection(source[0], formDesignVersionId, sourceElementPath);
                    if (null != compiledDocumentRule)
                    {
                        SourceHandlerDBManager dbManager = new SourceHandlerDBManager(1, _folderVersionServices, formDataInstanceManager, _formDesignServices, _masterListService);
                        CurrentRequestContext requestContext = new CurrentRequestContext();
                        foreach (var item in compiledDocumentRule)
                        {
                            RuleExecution ruleExecution = new RuleExecution(base.CurrentUserId, item, formInstanceId, folderVersionId, dbManager, details, _formDesignServices, _formInstanceService, requestContext);
                            string[] TargetPath = item.Target.TargetPath.Split('[');
                            if (TargetPath.Length > 0)
                            {
                                string dataLink = TargetPath[1].Replace("]", "");
                                sourceData.Add(new ExpressionBuilderViewModel
                                {
                                    TargetPath = dataLink,
                                    Data = ruleExecution.ProcessRule()
                                });
                            }
                        }
                    }
                }
            }
            return Json(JsonConvert.SerializeObject(sourceData), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult GetRuntimeEBResult(string sourceElementPath, int folderVersionId, int formInstanceId, int formDesignVersionId, string [] paramArray)
        {
            JToken sourceData = null;
            FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(1, formDesignVersionId, _formDesignServices);
            FormDesignVersionDetail details = formDesignVersionMgr.GetFormDesignVersion(true);
            ElementDocumentRuleViewModel ruleModel = _mlcService.GetElementDocumentRule(formDesignVersionId, sourceElementPath);
            FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(1, base.CurrentUserId, _formInstanceDataServices, base.CurrentUserName, _folderVersionServices);

            var getJson = ruleModel.RuleJSON;
            Documentrule documentRule = DocumentRuleSerializer.Deserialize(getJson);
            DocumentRuleCompiler ruleCompiler = new DocumentRuleCompiler(0, documentRule);
            CompiledDocumentRule compiledDocumentRule = ruleCompiler.CompileRule();
            SourceHandlerDBManager dbManager = new SourceHandlerDBManager(1, _folderVersionServices, formDataInstanceManager, _formDesignServices, _masterListService);
            CurrentRequestContext requestContext = new CurrentRequestContext();
            if (compiledDocumentRule != null)
            {
                RuleExecution ruleExecution = new RuleExecution(base.CurrentUserId, compiledDocumentRule, formInstanceId, folderVersionId, dbManager, details, _formDesignServices, _formInstanceService, requestContext);
                sourceData = ruleExecution.ProcessRule(paramArray);
            }

            return Json(JsonConvert.SerializeObject(sourceData), JsonRequestBehavior.AllowGet);
        }
		[HttpPost]
        [ValidateInput(false)]
        public JsonResult ProcessEBSRuleForSameSection(int folderVersionId, int formInstanceId, int formDesignVersionId, string sourceElementPath, string[] ElementValue, bool isMultiselect, string sectionData)
        {
            List<ExpressionBuilderViewModel> sourceData = new List<ExpressionBuilderViewModel>();
            FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(1, formDesignVersionId, _formDesignServices);
            FormDesignVersionDetail details = formDesignVersionMgr.GetFormDesignVersion(true);
            FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(1, base.CurrentUserId, _formInstanceDataServices, base.CurrentUserName, _folderVersionServices);
            {
                string[] source = sourceElementPath.Split('.');
                if (!string.IsNullOrEmpty(sectionData))
                {
                    try
                    {
                        sectionData = _formInstanceDataServices.AddNodeToSectionData(source[0], sectionData);
                    }
                    catch (Exception ex)
                    {
                    }
                    formDataInstanceManager.SetCacheData(formInstanceId, source[0], sectionData);
                    List<ruleinterpreter.model.CompiledDocumentRule> compiledDocumentRule = _uiElementService.GetAllDocumentRuleBySection(source[0], formDesignVersionId, sourceElementPath);
                    if (null != compiledDocumentRule)
                    {
                        SourceHandlerDBManager dbManager = new SourceHandlerDBManager(1, _folderVersionServices, formDataInstanceManager, _formDesignServices, _masterListService);
                        CurrentRequestContext requestContext = new CurrentRequestContext();
                        foreach (var item in compiledDocumentRule)
                        {
                            RuleExecution ruleExecution = new RuleExecution(base.CurrentUserId, item, formInstanceId, folderVersionId, dbManager, details, _formDesignServices, _formInstanceService, requestContext);
                            string[] TargetPath = item.Target.TargetPath.Split('[');
                            if (TargetPath.Length > 0)
                            {
                                string dataLink = TargetPath[1].Replace("]", "");
                                JToken RulesOutput = null;
                                RulesOutput = ruleExecution.ProcessRule();
                                sourceData.Add(new ExpressionBuilderViewModel
                                {
                                    TargetPath = dataLink,
                                    Data= RulesOutput
                                });
                                if (RulesOutput != null)
                                {
                                    if (!string.IsNullOrEmpty(sectionData))
                                    {
                                        JToken sectionDataObject = JToken.Parse(sectionData);
                                        sectionDataObject.SelectToken(dataLink).Replace(RulesOutput);
                                        string updatedSectionData = sectionDataObject.ToString(Newtonsoft.Json.Formatting.None);
                                        formDataInstanceManager.SetCacheData(formInstanceId, source[0], updatedSectionData);
                                    }
                                }

                            }
                            
                        }
                    }
                }
            }
            return Json(JsonConvert.SerializeObject(sourceData), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ProcessRuleForSameSectionVBIDRxCalculation(int folderVersionId, int formInstanceId, int formDesignVersionId, string sourceElementPath, string[] ElementValue, bool isMultiselect, string sectionData)
        {
            List<ExpressionBuilderViewModel> sourceData = new List<ExpressionBuilderViewModel>();
            FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(1, formDesignVersionId, _formDesignServices);
            FormDesignVersionDetail details = formDesignVersionMgr.GetFormDesignVersion(true);
            FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(1, base.CurrentUserId, _formInstanceDataServices, base.CurrentUserName, _folderVersionServices);
            {
                string[] source = sourceElementPath.Split('.');
                if (!string.IsNullOrEmpty(sectionData))
                {
                    try
                    {
                        sectionData = _formInstanceDataServices.AddNodeToSectionData(source[0], sectionData);
                    }
                    catch (Exception ex)
                    {
                    }
                    formDataInstanceManager.SetCacheData(formInstanceId, source[0], sectionData);
                    List<ruleinterpreter.model.CompiledDocumentRule> compiledDocumentRule = _uiElementService.GetAllDocumentRuleBySection(source[0], formDesignVersionId, sourceElementPath);
                    if (null != compiledDocumentRule)
                    {
                        SourceHandlerDBManager dbManager = new SourceHandlerDBManager(1, _folderVersionServices, formDataInstanceManager, _formDesignServices, _masterListService);
                        CurrentRequestContext requestContext = new CurrentRequestContext();
                        foreach (var item in compiledDocumentRule)
                        {
                            RuleExecution ruleExecution = new RuleExecution(base.CurrentUserId, item, formInstanceId, folderVersionId, dbManager, details, _formDesignServices, _formInstanceService, requestContext);
                            string[] TargetPath = item.Target.TargetPath.Split('[');
                            if (TargetPath.Length > 0)
                            {
                                string dataLink = TargetPath[1].Replace("]", "");
                                sourceData.Add(new ExpressionBuilderViewModel
                                {
                                    TargetPath = dataLink,
                                    Data = ruleExecution.ProcessRule()
                                });
                            }
                        }
                    }
                }
            }
            return Json(JsonConvert.SerializeObject(sourceData), JsonRequestBehavior.AllowGet);
        }
		
		[ValidateInput(false)]
        public JsonResult ProcessRuleForRepeater(int tenantId)
        {
            //To Do: Use this method for processing the repeaters rules at server side.
            return Json("");
        }

        public JsonResult GetDataForTest(int tenantId, string colModel, int formInstanceId, int formDesignVersionId)
        {
            JArray columns = JsonConvert.DeserializeObject<JArray>(colModel);
            List<ProductTestDataModel> operands = new List<ProductTestDataModel>();
            foreach (var col in columns)
            {
                string colName = col["name"].ToString();
                if (colName.Contains("#!"))
                {
                    operands.Add(new ProductTestDataModel()
                    {
                        Label = colName.Substring(colName.IndexOf("#") + 2, (colName.IndexOf("!#") - colName.IndexOf("#!")) - 2),
                        UIElementName = colName.Substring(0, colName.IndexOf("#"))
                    });
                }
            }
            string formData = _folderVersionServices.GetFormInstanceDataCompressed(tenantId, formInstanceId);
            if (!String.IsNullOrEmpty(formData))
            {
                JObject data = JObject.Parse(formData);
                operands = _rulesManagerService.GetOperandNames(tenantId, formDesignVersionId, operands);
                foreach (var item in operands)
                {
                    var tokenValue = data.SelectToken(item.UIElementFullName);
                    if (tokenValue.Type == JTokenType.Array)
                    {
                        item.Value = String.Join(",", tokenValue);
                    }
                    else
                    {
                        item.Value = tokenValue.ToString();
                    }
                }
            }
            return Json(operands, JsonRequestBehavior.AllowGet);
        }
    }
}