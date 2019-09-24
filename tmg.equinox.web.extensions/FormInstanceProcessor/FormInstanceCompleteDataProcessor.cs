using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.ConsumerAccount;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.web.ExpresionBuilder;
using tmg.equinox.web.FormInstance;
using tmg.equinox.web.FormInstanceManager;
using tmg.equinox.web.Framework.Caching;
using System.Threading.Tasks;

namespace tmg.equinox.web.FormInstanceProcessor
{
    public class FormInstanceCompleteDataProcessor : IDataProcessor
    {
        private int _tenantId = 1;
        private int _formInstanceId;
        private int _folderVersionId;
        private int _formDesignVersionId;
        private bool _isFolderReleased;
        private IFolderVersionServices _folderVersionServices;
        private IFormInstanceDataServices _formInstanceDataServices;
        private IUIElementService _uiElementService;
        private int? _currentUserId;
        private FormInstanceDataManager _formDataInstanceManager;
        private string _sectionName;
        private FormDesignVersionDetail _detail;
        private string _currentUserName;
        private IFormDesignService _formDesignServices;
        private IMasterListService _masterListService;
        private IFormInstanceService _formInstanceService;
        private string _previousSectionData;
        private bool _loadAllSection = false;
        private IFormInstanceRuleExecutionLogService _ruleExecutionLogService;
        private int _ruleExecutionLogParentRowID;
        public FormInstanceCompleteDataProcessor(IFolderVersionServices folderVersionServices, int formInstanceId, int folderVersionId, int formDesignVersionId, bool isFolderReleased, int? userId, FormInstanceDataManager formDataInstanceManager, IFormInstanceDataServices formInstanceDataServices, IUIElementService uiElementService, FormDesignVersionDetail detail, string currentUserName, IFormDesignService formDesignServices, IMasterListService masterListService, IFormInstanceService formInstanceService, IFormInstanceRuleExecutionLogService ruleExecutionLogService)
        {
            this._formInstanceId = formInstanceId;
            this._folderVersionId = folderVersionId;
            this._formDesignVersionId = formDesignVersionId;
            this._isFolderReleased = isFolderReleased;
            this._folderVersionServices = folderVersionServices;
            this._uiElementService = uiElementService;
            this._currentUserId = userId;
            this._formDataInstanceManager = formDataInstanceManager;
            this._formInstanceDataServices = formInstanceDataServices;
            this._detail = detail;
            this._currentUserName = currentUserName;
            this._formDesignServices = formDesignServices;
            this._masterListService = masterListService;
            this._formInstanceService = formInstanceService;
            this._loadAllSection = LoadCompleteFormData();
            this._ruleExecutionLogService = ruleExecutionLogService;
        }

        public bool LoadCompleteFormData()
        {
            bool result = false;
            var settings = ConfigurationManager.AppSettings["LoadAllSections"];
            if (!string.IsNullOrEmpty(settings) && Convert.ToBoolean(settings))
            {
                result = true;
            }

            return result;
        }

        public void RunProcessorOnDocumentLoad(string sectionName, bool isRefreshCache)
        {
            IDataCacheHandler cacheHandler = DataCacheHandlerFactory.GetHandler();
            _detail.JSONData = cacheHandler.Get(_tenantId, _formInstanceId, false, _detail, _folderVersionServices, _currentUserId);

            if (isRefreshCache == true)
            {
                _formDataInstanceManager.RemovePartilCacheDataForSOTView(_formInstanceId, _detail);
            }
            this.ExecuteProcessorOnSectionLoad(_detail.JSONData);
            this.ExecuteProcessorOnDocumentLoad();

        }

        public void RunProcessorOnSectionLoad(string sectionName, bool isRefreshCache)
        {
            _sectionName = sectionName;

            _detail.JSONData = _formDataInstanceManager.GetSectionData(_formInstanceId, _sectionName, isRefreshCache, _detail, false, false);

            this.ExecuteProcessorOnSectionLoad(_detail.JSONData);

        }

        public void RunProcessorOnSectionSave(string sectionName, bool isRefreshCache, string currentSectionData, string previousSectionData)
        {
            _sectionName = sectionName;
            _previousSectionData = previousSectionData;
            _detail.JSONData = currentSectionData;

            this.ExecuteProcessorOnSectionSave(_detail.JSONData);
        }

        public void RunProcessorOnDocumentSave(string sectionName, bool isRefreshCache, string currentSectionData, string previousSectionData)
        {
            _sectionName = sectionName;
            _previousSectionData = previousSectionData;
            _detail.JSONData = currentSectionData;

            this.ExecuteProcessorOnDocumentSave(_detail.JSONData);
        }

        public void RunViewProcessorsOnCollateralGeneration()
        {
            this.ProcessCollateralViewDocumentRules();
        }

        public void RunViewProcessorsOnAnchorSave()
        {
            this.ProcessViewDocumentRules();
        }


        public void ExecuteProcessorOnDocumentLoad()
        {
            this.ProcessCustomRuleOnDocumentLoad();
        }

        private void ExecuteProcessorOnSectionLoad(string sectionData)
        {
            //this.SaveRuleExecutionLogEntry();

            this.ProcessDataSource();
            this.ProcessConfigureRule();
            //this.ProcessSectionLoadRules();
            //this.ProcessCustomRuleOnSectionLoad();
            this.ProcessSectionVisibleRules();
            this.ProcessConfigureVisibleRule();
        }

        private void SaveRuleExecutionLogEntry()
        {
            var sectionDesign = _detail.Sections.Where(s => s.GeneratedName == _sectionName).FirstOrDefault();
            string sectionLabel = _sectionName;
            int sectionElementID = -1;
            if (sectionDesign != null)
            {
                sectionLabel = sectionDesign.Label;
                sectionElementID = sectionDesign.ID;
            }
            //_ruleExecutionLogParentRowID = _ruleExecutionLogService.SaveFormInstanceRuleExecutionServerlogDataOnLoad(_formInstanceId, sectionElementID, sectionLabel);
        }

        private void ExecuteProcessorOnSectionSave(string sectionData)
        {
            this.ProcessExecutionBuilderRules();
            this.ProcessCustomRuleOnSectionSave();
        }

        private void ExecuteProcessorOnDocumentSave(string sectionData)
        {
            //this.ProcessExecutionBuilderRules();
            this.ProcessDocumentRules();
            //this.ProcessCustomRuleOnSectionSave();
        }

        private void ProcessVersionSync(string sectionData)
        {
            string defaultJSONData = _detail.GetDefaultJSONDataObject();
            VersionSyncProcessor syncProc = new VersionSyncProcessor(_sectionName, defaultJSONData, sectionData);
            _detail.JSONData = syncProc.Run();
        }

        private void ProcessDataSource()
        {
            if (_detail.DataSources != null && _detail.DataSources.Count > 0)
            {
                DataSourceProcessor dsProcessor = new DataSourceProcessor(_tenantId, _formInstanceId, _folderVersionId, _detail, _formDesignVersionId, _isFolderReleased, _folderVersionServices, _formDataInstanceManager, _sectionName, this._formDesignServices);
                dsProcessor.Run();
            }
        }

        private void ProcessSectionVisibleRules()
        {
            try
            {
                List<SectionDesign> sectionList = _detail.Sections;
                if (sectionList != null)
                {
                    foreach (var section in sectionList)
                    {
                        var sectionDesign = section;// _detail.Sections.Where(s => s.GeneratedName == _sectionName).FirstOrDefault();
                        if (sectionDesign != null)
                        {
                            var subSectionElements = sectionDesign.Elements.Where(s => s.Section != null).Select(a => a.ElementID).ToList();
                            subSectionElements.Add(sectionDesign.ID);

                            List<RuleDesign> sectionRules = _detail.Rules.Where(a => subSectionElements.Contains(a.UIELementID) && a.TargetPropertyTypeId == 4).ToList();
                            if (sectionRules.Count > 0)
                            {
                                ConfigureRuleProcessor ruleProcessor = new ConfigureRuleProcessor(_detail, _formDataInstanceManager, section.GeneratedName, _formInstanceId, sectionRules, _folderVersionServices, _formDesignServices, _formInstanceService,_ruleExecutionLogService, _ruleExecutionLogParentRowID);
                                ruleProcessor.RunRules();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void ProcessConfigureVisibleRule()
        {
            try
            {
                List<SectionDesign> sectionList = _detail.Sections;
                if (sectionList != null)
                {
                    foreach (var section in sectionList)
                    {
                        List<RuleDesign> sectionRules = _detail.Rules.Where(a => a.UIElementFullName.Split('.')[0].ToString() == section.GeneratedName
                && a.RunOnLoad.Equals(true) && a.TargetPropertyTypeId == 4
                ).ToList();

                        if (sectionRules.Count > 0)
                        {
                            ConfigureRuleProcessor ruleProcessor = new ConfigureRuleProcessor(_detail, _formDataInstanceManager, section.GeneratedName, _formInstanceId, sectionRules,_folderVersionServices,_formDesignServices,_formInstanceService,_ruleExecutionLogService, _ruleExecutionLogParentRowID);
                            ruleProcessor.RunVisibleRules();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }


        private void ProcessConfigureRule()
        {
            List<RuleDesign> sectionRules = _detail.Rules.Where(a => a.RunOnLoad.Equals(true)).ToList();
            if (sectionRules.Count > 0)
            {
                ConfigureRuleProcessor ruleProcessor = new ConfigureRuleProcessor(_detail, _formDataInstanceManager, _sectionName, _formInstanceId, sectionRules, _folderVersionServices, _formDesignServices, _formInstanceService,_ruleExecutionLogService, _ruleExecutionLogParentRowID);
                ruleProcessor.Run();
            }
        }

        private void ProcessExecutionBuilderRules()
        {
            ExpressionBuilderRuleProcessor proc = new ExpressionBuilderRuleProcessor(_currentUserId, _tenantId, _formInstanceId, _folderVersionId, _detail, _formDesignVersionId, _isFolderReleased, _folderVersionServices, _uiElementService, _formDataInstanceManager, _formDesignServices, _sectionName, _previousSectionData, _detail.JSONData, _masterListService, _formInstanceService);
            proc.ProcessSectionSave(_sectionName);
        }

        public void ProcessDocumentRulesAsync()
        {
            List<string> getVisitedSections = GetUpdatedSections();
            ExpressionBuilderRuleProcessor proc = new ExpressionBuilderRuleProcessor(_currentUserId, _tenantId, _formInstanceId, _folderVersionId, _detail, _formDesignVersionId, _isFolderReleased, _folderVersionServices, _uiElementService, _formDataInstanceManager, _formDesignServices, _sectionName, _previousSectionData, _detail.JSONData, _masterListService, _formInstanceService);
            Task.Run(() => proc.ProcessDocumentSave(getVisitedSections));
        }

        private void ProcessDocumentRules()
        {
            List<string> getVisitedSections = GetUpdatedSections();
            ExpressionBuilderRuleProcessor proc = new ExpressionBuilderRuleProcessor(_currentUserId, _tenantId, _formInstanceId, _folderVersionId, _detail, _formDesignVersionId, _isFolderReleased, _folderVersionServices, _uiElementService, _formDataInstanceManager, _formDesignServices, _sectionName, _previousSectionData, _detail.JSONData, _masterListService, _formInstanceService);
            proc.ProcessDocumentSave(getVisitedSections);
            proc.SaveExpressionRuleActivityLogData(_currentUserName);
        }

        private List<string> GetUpdatedSections()
        {
            List<string> visitedSections = new List<string>();
            JObject currentFormData = JObject.Parse(_detail.JSONData);
            JObject prevFormData = JObject.Parse(_previousSectionData);
            foreach (var section in currentFormData)
            {
                if (!JToken.DeepEquals(section.Value, prevFormData[section.Key]))
                {
                    visitedSections.Add(section.Key);
                }
            }

            return visitedSections;
        }

        private void ProcessViewDocumentRules()
        {
            ExpressionBuilderRuleProcessor proc = new ExpressionBuilderRuleProcessor(_currentUserId, _tenantId, _formInstanceId, _folderVersionId, _detail, _formDesignVersionId, _isFolderReleased, _folderVersionServices, _uiElementService, _formDataInstanceManager, _formDesignServices, _sectionName, _previousSectionData, _detail.JSONData, _masterListService, _formInstanceService);
            proc.ProcessViewDocumentRules();

        }

        private void ProcessSectionLoadRules()
        {
            ExpressionBuilderRuleProcessor proc = new ExpressionBuilderRuleProcessor(_currentUserId, _tenantId, _formInstanceId, _folderVersionId, _detail, _formDesignVersionId, _isFolderReleased, _folderVersionServices, _uiElementService, _formDataInstanceManager, _formDesignServices, _sectionName, _previousSectionData, _detail.JSONData, _masterListService, _formInstanceService);
            proc.ProcessSectionLoad(_sectionName);
        }

        private void ProcessCollateralViewDocumentRules()
        {
            ExpressionBuilderRuleProcessor proc = new ExpressionBuilderRuleProcessor(_currentUserId, _tenantId, _formInstanceId, _folderVersionId, _detail, _formDesignVersionId, _isFolderReleased, _folderVersionServices, _uiElementService, _formDataInstanceManager, _formDesignServices, _sectionName, _previousSectionData, _detail.JSONData, _masterListService, _formInstanceService);
            proc.ProcessCollateralViewDocumentRules();
        }

        private void ProcessCustomRuleOnSectionLoad()
        {
            IExpressionBuilderRuleHandler handler = ExpressionBuilderRuleFactory.GetHandler(_formDataInstanceManager, _detail.FormDesignId, _detail, _formInstanceId, _formDesignServices, _folderVersionServices,this._folderVersionId);
            if (handler != null)
            {
                handler.RunRulesOnSectionLoad(_sectionName, _formInstanceService);
            }
        }

        private void ProcessCustomRuleOnDocumentLoad()
        {
            IExpressionBuilderRuleHandler handler = ExpressionBuilderRuleFactory.GetHandler(_formDataInstanceManager, _detail.FormDesignId, _detail, _formInstanceId, _formDesignServices, _folderVersionServices, this._folderVersionId);
            if (handler != null)
            {
                handler.RunRulesOnDocumentLoad(_formInstanceService, _folderVersionServices, _formDesignServices, _folderVersionId);
            }
        }

        private void ProcessCustomRuleOnSectionSave()
        {
            IExpressionBuilderRuleHandler handler = ExpressionBuilderRuleFactory.GetHandler(_formDataInstanceManager, _detail.FormDesignId, _detail, _formInstanceId, _formDesignServices, _folderVersionServices, this._folderVersionId);
            if (handler != null && HasDataChanged.hasDataChanged(_detail.JSONData, _previousSectionData))
            {
                handler.RunRulesOnSectionSave(_sectionName, _previousSectionData, _folderVersionServices, _formDesignServices, _folderVersionId);
            }
        }

        public ServiceResult UpdateAccountProductMap(int folderId, int folderVersionId, IConsumerAccountService consumerAccountService, ServiceResult result)
        {
            //ServiceResult result = null;
            string productId = "", productType = "", planCode = "", rXBenefit = "", sNPType = "";
            string serviceGroup = string.Empty;
            string anocChartPlanType = string.Empty;
            string productName = string.Empty;
            //Get AccountPropertyName and AccountPropertyPaths
            IEnumerable<FormDesignAccountViewModel> FormDesignAccountList = consumerAccountService.GetFormDesignAccountMapping(_detail.FormDesignVersionId);


            //if FormDesignAccountList is not null then adding or updating to account product map table
            if (FormDesignAccountList.Count() != 0)
            {

                IDataCacheHandler handler = DataCacheHandlerFactory.GetHandler();
                string formData = handler.Get(1, _formInstanceId, false, _detail, _folderVersionServices, _currentUserId);

                FormInstanceParser formInstanceParser = new FormInstanceParser();
                IEnumerable<FormDesignAccountViewModel> sectionFormDesignAccountList = FormDesignAccountList.ToList();

                if (sectionFormDesignAccountList.Count() > 0)
                {
                    foreach (var proertypathsearch in sectionFormDesignAccountList)
                    {
                        //Get the value of product name and product type using AccountPropertyPath
                        var value = formInstanceParser.GetValue(formData, proertypathsearch.PropertyPath);
                        if (value is string)
                        {
                            switch (proertypathsearch.PropertyName)
                            {
                                case "ContractNumber":
                                    productId = value.ToString();
                                    break;
                                case "PlanName":
                                    productType = value.ToString();
                                    break;
                                case "PlanCode":
                                    planCode = value.ToString();
                                    break;
                                case "ANOCChartPlanType":
                                    anocChartPlanType = value.ToString();
                                    break;
                                case "RXBenefit":
                                    rXBenefit = value.ToString();
                                    if (rXBenefit != null && rXBenefit != "" && (rXBenefit.ToUpper() == "YES" || rXBenefit.ToUpper() == "TRUE"))
                                    {
                                        rXBenefit = "RX";
                                    }
                                    else
                                    {
                                        rXBenefit = "No RX";
                                    }
                                    break;
                                case "SNPType":
                                    sNPType = value.ToString();
                                    if (sNPType == "Institutional")
                                    {
                                        sNPType = "ISNP";
                                    }
                                    else if (sNPType == "Dual-Eligible")
                                    {
                                        sNPType = "DSNP";
                                    }
                                    else if (sNPType == "Chronic Or Disabling Condition")
                                    {
                                        sNPType = "CSNP";
                                    }
                                    else if (sNPType == null || sNPType == "")
                                    {
                                        sNPType = "No SNP";
                                    }
                                    break;
                            }
                        }
                    }
                }

                result = consumerAccountService.UpdateAccountProductMap(_tenantId, _formInstanceId, folderId, folderVersionId, productType, productId, planCode, _currentUserName, serviceGroup, productName, anocChartPlanType, rXBenefit, sNPType);
                result = UpdatePBPDetails(folderId, folderVersionId, productType, productId, consumerAccountService, result);
            }

            return result;
        }

        public ServiceResult UpdatePBPDetails(int folderId, int folderVersionId, string planName, string planNumber, IConsumerAccountService consumerAccountService, ServiceResult result)
        {
            int docid = 0;
            docid = _formInstanceService.GetDocID(_formInstanceId);
            result = consumerAccountService.UpdatePBPImportDetailsMap(_tenantId, _formInstanceId, folderId, folderVersionId, planName, planNumber, docid, _currentUserName);
            return result;

        }

        public void RunPreExitValidateRules(string sectionName, bool isRefreshCache)
        {
            throw new NotImplementedException();
        }

        public bool RunSectionVisibleRule(string sectionName)
        {
            throw new NotImplementedException();
        }
    }
}