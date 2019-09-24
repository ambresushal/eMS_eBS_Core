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
using tmg.equinox.web.FormInstanceProcessor.SourceTargetDataManager.SourceHandler;
using tmg.equinox.web.FormDesignManager;

namespace tmg.equinox.web.FormInstanceProcessor
{
    public class FormInstanceDataProcessor : IDataProcessor
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
        private string _ancillaryProductName;
        public FormInstanceDataProcessor(IFolderVersionServices folderVersionServices, int formInstanceId, int folderVersionId, int formDesignVersionId, bool isFolderReleased, int? userId, FormInstanceDataManager formDataInstanceManager, IFormInstanceDataServices formInstanceDataServices, IUIElementService uiElementService, FormDesignVersionDetail detail, string currentUserName, IFormDesignService formDesignServices, IMasterListService masterListService, IFormInstanceService formInstanceService)
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
        }
        public FormInstanceDataProcessor(IFolderVersionServices folderVersionServices, int formInstanceId, int folderVersionId, int formDesignVersionId, bool isFolderReleased, int? userId, FormInstanceDataManager formDataInstanceManager, IFormInstanceDataServices formInstanceDataServices, IUIElementService uiElementService, FormDesignVersionDetail detail, string currentUserName, IFormDesignService formDesignServices, IMasterListService masterListService, IFormInstanceService formInstanceService, IFormInstanceRuleExecutionLogService ruleExecutionLogService, string ancillaryProductName = "")
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
            this._ancillaryProductName = ancillaryProductName;
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
            _sectionName = sectionName;

            _detail.JSONData = _formDataInstanceManager.GetSectionData(_formInstanceId, _sectionName, isRefreshCache, _detail, false, false);

            this.ExecuteProcessorOnSectionLoad(_detail.JSONData);
            //this.ExecuteProcessorOnDocumentLoad();

            if (this._loadAllSection)
            {
                IDataCacheHandler cacheHandler = DataCacheHandlerFactory.GetHandler();
                string formData = cacheHandler.Get(_tenantId, _formInstanceId, true, _detail, _folderVersionServices, _currentUserId);
                JObject objFormData = JObject.Parse(formData);
                objFormData[sectionName] = JObject.Parse(_detail.JSONData)[sectionName];
                _detail.JSONData = objFormData.ToString();
            }
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

        public void RunPreExitValidateRules(string sectionName,bool isRefreshCache)
        {
            _sectionName = sectionName;
            _detail.JSONData = _formDataInstanceManager.GetSectionData(_formInstanceId, _sectionName, isRefreshCache, _detail, false, false);
            this.ExecuteExitValidateRules();
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
            //if (_ruleExecutionLogService != null)
            //    this.SaveRuleExecutionLogEntry();

            this.ProcessDataSource();
            this.ProcessConfigureRule();
            this.ProcessSectionLoadRules();
            this.ProcessCustomRuleOnSectionLoad();
            this.ProcessSectionVisibleRules();
            this.ProcessConfigureVisibleRule();
            if (tmg.equinox.config.Config.GetApplicationName().ToLower() == "ebenefitsync" && _detail.IsMasterList == false)
            {
                this.ProcessAncillaryDocument();
            }
        }

        private void ProcessAncillaryDocument()
        {
            AncillaryProcessor processor = new AncillaryProcessor(_detail, _formDataInstanceManager, _sectionName, _formInstanceId, _folderVersionId, _folderVersionServices, _formDesignServices, _formInstanceService, _ancillaryProductName);
            if (processor.IsAncillarySection())
            {
                processor.GetAncillaryProductList();
            }
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
           // _ruleExecutionLogParentRowID = _ruleExecutionLogService.SaveFormInstanceRuleExecutionServerlogDataOnLoad(_formInstanceId, sectionElementID, sectionLabel);
        }

        private void ExecuteExitValidateRules()
        {
            this.ProcessSectionVisibleRules();
            this.ProcessConfigureVisibleRule();
        }

        private void ExecuteProcessorOnSectionSave(string sectionData)
        {
            this.ProcessExecutionBuilderRules();
            this.ProcessCustomRuleOnSectionSave();
        }

        private void ExecuteProcessorOnDocumentSave(string sectionData)
        {
            this.ProcessExecutionBuilderRules();
            this.ProcessDocumentRules();
            this.ProcessCustomRuleOnSectionSave();
        }

        private void ProcessVersionSync(string sectionData)
        {
            string defaultJSONData = _detail.GetDefaultJSONDataObject();
            VersionSyncProcessor syncProc = new VersionSyncProcessor(_sectionName, defaultJSONData, sectionData);
            _detail.JSONData = syncProc.Run();
        }

        private void ProcessDataSource()
        {
            try
            {
                if (_detail.DataSources != null && _detail.DataSources.Count > 0)
                {
                    DataSourceProcessor dsProcessor = new DataSourceProcessor(_tenantId, _formInstanceId, _folderVersionId, _detail, _formDesignVersionId, _isFolderReleased, _folderVersionServices, _formDataInstanceManager, _sectionName, this._formDesignServices);
                    dsProcessor.Run();
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
        }

        private void ProcessSectionVisibleRules()
        {
            var sectionDesign = _detail.Sections.Where(s => s.GeneratedName == _sectionName).FirstOrDefault();
            if (sectionDesign != null)
            {
                var subSectionElements = sectionDesign.Elements.Where(s => s.Section != null).Select(a => a.ElementID).ToList();
                subSectionElements.Add(sectionDesign.ID);

                List<RuleDesign> sectionRules = _detail.Rules.Where(a => subSectionElements.Contains(a.UIELementID) && a.TargetPropertyTypeId == 4).ToList();
                if (sectionRules.Count > 0)
                {
                    ConfigureRuleProcessor ruleProcessor = new ConfigureRuleProcessor(_detail, _formDataInstanceManager, _sectionName, _formInstanceId, sectionRules, _folderVersionServices, _formDesignServices, _formInstanceService, _ruleExecutionLogService, _ruleExecutionLogParentRowID);
                    ruleProcessor.RunRules();
                }
            }
        }

        private void ProcessConfigureVisibleRule()
        {
            List<RuleDesign> sectionRules = _detail.Rules.Where(a => a.UIElementFullName.Split('.')[0].ToString() == _sectionName
             && a.TargetPropertyTypeId == 4
            ).ToList();

            if (sectionRules.Count > 0)
            {
                ConfigureRuleProcessor ruleProcessor = new ConfigureRuleProcessor(_detail, _formDataInstanceManager, _sectionName, _formInstanceId, sectionRules, _folderVersionServices, _formDesignServices, _formInstanceService, _ruleExecutionLogService, _ruleExecutionLogParentRowID);
                ruleProcessor.RunVisibleRules();
            }
        }

        private void ProcessConfigureRule()
        {
            //GenerateRuleDescriptions(_detail.Rules);
            //GenerateValidationDescriptions(_detail.Validations);
            List<RuleDesign> sectionRules = _detail.Rules.Where(a => a.UIElementFullName.Split('.')[0].ToString() == _sectionName
            && a.RunOnLoad.Equals(true)
            ).ToList();

            if (sectionRules.Count > 0)
            {
                //this is for given section load, so add parent entry for section load and get parent id pass for further process.
                ConfigureRuleProcessor ruleProcessor = new ConfigureRuleProcessor(_detail, _formDataInstanceManager, _sectionName, _formInstanceId, sectionRules, _folderVersionServices, _formDesignServices, _formInstanceService, _ruleExecutionLogService, _ruleExecutionLogParentRowID);
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
            if (ExpressionBuilderRuleFactory.IsAnchor(_detail.FormDesignId))
            {
                List<string> updatedSections = GetUpdatedSections();
                ExpressionBuilderRuleProcessor proc = new ExpressionBuilderRuleProcessor(_currentUserId, _tenantId, _formInstanceId, _folderVersionId, _detail, _formDesignVersionId, _isFolderReleased, _folderVersionServices, _uiElementService, _formDataInstanceManager, _formDesignServices, _sectionName, _previousSectionData, _detail.JSONData, _masterListService, _formInstanceService);
                Task.Run(() => proc.ProcessDocumentSave(updatedSections));
            }
        }

        private void ProcessDocumentRules()
        {
            if (ExpressionBuilderRuleFactory.IsAnchor(_detail.FormDesignId))
            {
                List<string> updatedSections = GetUpdatedSections();
                ExpressionBuilderRuleProcessor proc = new ExpressionBuilderRuleProcessor(_currentUserId, _tenantId, _formInstanceId, _folderVersionId, _detail, _formDesignVersionId, _isFolderReleased, _folderVersionServices, _uiElementService, _formDataInstanceManager, _formDesignServices, _sectionName, _previousSectionData, _detail.JSONData, _masterListService, _formInstanceService);
                proc.ProcessDocumentSave(updatedSections);
                proc.SaveExpressionRuleActivityLogData(_currentUserName);
            }
        }

        private List<string> GetUpdatedSections()
        {
            List<string> visitedSections = new List<string>();
            IDataCacheHandler cacheHandler = DataCacheHandlerFactory.GetHandler();
            string formData = cacheHandler.Get(_tenantId, _formInstanceId, true, _detail, _folderVersionServices, _currentUserId);
            JObject prevFormData = JObject.Parse(formData);

            FormInstanceSectionDataCacheHandler handler = new FormInstanceSectionDataCacheHandler();
            List<JToken> sectionLists = handler.GetSectionListFromCache(_formInstanceId, _currentUserId);

            foreach (var section in sectionLists)
            {
                JObject cacheSectionData = JObject.Parse(handler.IsExists(_formInstanceId, section.ToString(), _currentUserId));
                if (!JToken.DeepEquals(cacheSectionData[section.ToString()], prevFormData[section.ToString()]))
                {
                    visitedSections.Add(section.ToString());
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
            IExpressionBuilderRuleHandler handler = ExpressionBuilderRuleFactory.GetHandler(_formDataInstanceManager, _detail.FormDesignId, _detail, _formInstanceId, _formDesignServices, _folderVersionServices, this._folderVersionId);
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
            string productId = "", productType = "", planCode = "", anocChartPlanType = string.Empty, rXBenefit = "", sNPType = ""; ;
            string serviceGroup = string.Empty;
            string productName = string.Empty;
            string secData = "";

            //Get AccountPropertyName and AccountPropertyPaths
            IEnumerable<FormDesignAccountViewModel> FormDesignAccountList = consumerAccountService.GetFormDesignAccountMapping(_detail.FormDesignVersionId);


            //if FormDesignAccountList is not null then adding or updating to account product map table
            if (FormDesignAccountList.Count() != 0)
            {
                FormInstanceSectionDataCacheHandler handler = new FormInstanceSectionDataCacheHandler();
                List<JToken> sectionLists = handler.GetSectionListFromCache(_formInstanceId, _currentUserId);

                FormInstanceParser formInstanceParser = new FormInstanceParser();
                foreach (JToken sectionName in sectionLists)
                {
                    IEnumerable<FormDesignAccountViewModel> sectionFormDesignAccountList = FormDesignAccountList.Where(a => a.PropertyPath.Split('.')[0].ToString() == sectionName.ToString()).ToList();

                    if (sectionFormDesignAccountList.Count() > 0)
                    {
                        secData = _formDataInstanceManager.GetSectionData(_formInstanceId, sectionName.ToString(), false, _detail, false, false);
                        foreach (var proertypathsearch in sectionFormDesignAccountList)
                        {
                            //Get the value of product name and product type using AccountPropertyPath
                            var value = formInstanceParser.GetValue(secData, proertypathsearch.PropertyPath);
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
                                    case "CSRVariationType":
                                        planCode = value.ToString();
                                        break;
                                    case "ANOCChartPlanType":
                                    case "State":
                                        anocChartPlanType = value.ToString();
                                        break;
                                    case "PlanType":
                                        anocChartPlanType = value.ToString();
                                        break;
                                    case "QHPNonQHP":
                                        serviceGroup = value.ToString();
                                        break;
                                    case "MarketType":
                                        productName = value.ToString();
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

        private void GenerateValidationDescriptions(List<ValidationDesign> validations)
        {
            System.Diagnostics.Debug.WriteLine("UI Element\\tIs Required\\tMax Length\\tValidation Regex\\tMessage\\tMask Expression\\tPlaceholder");
            foreach (var validation in validations)
            {
                string element = validation.FullName;
                string required = "NO";
                if (validation.IsRequired == true)
                {
                    required = "YES";
                }
                string regex = validation.Regex;
                string maxLength = "";
                if (validation.HasMaxLength == true)
                {
                    maxLength = validation.MaxLength.ToString();
                }
                string maskExpression = validation.MaskExpression;
                string placeHolder = validation.PlaceHolder;
                string message = validation.ValidationMessage;
                System.Diagnostics.Debug.WriteLine(element + "\\t[][]" + required + "\\t[][]" + maxLength + "\\t[][]" + regex + "\\t[][]" + message + "\\t[][]" + maskExpression + "\\t[][]" + placeHolder);
            }
        }

        private void GenerateRuleDescriptions(List<RuleDesign> rules)
        {
            System.Diagnostics.Debug.WriteLine("Rule Target Element\\tExpression\\tTarget Property\\tSuccess Value\\tFailure Value\\tMessage\\t");
            foreach (var rule in rules)
            {
                string ruleOutput = rule.UIElementFullName;
                string expStr = "";
                if (rule.RootExpression != null)
                {
                    ExpressionDesign rootExp = rule.RootExpression;
                    ProcessNode(rootExp, ref expStr);
                }

                string targetProperty = "";
                switch (rule.TargetPropertyTypeId)
                {
                    case 1:
                        targetProperty = "Enabled";
                        break;
                    case 2:
                        targetProperty = "Run Validation";
                        break;
                    case 3:
                        targetProperty = "Value";
                        break;
                    case 4:
                        targetProperty = "Visible";
                        break;
                    case 5:
                        targetProperty = "Is Required";
                        break;
                    case 6:
                        targetProperty = "Error";
                        break;
                    case 7:
                        targetProperty = "Expression Value";
                        break;
                    default:
                        targetProperty = "NOT SET";
                        break;
                }
                string successValue = "";
                if (rule.IsResultSuccessElement == true)
                {
                    successValue = rule.SuccessValueFullName;
                }
                else
                {
                    successValue = rule.SuccessValue;
                }
                string failureValue = "";
                if (rule.IsResultFailureElement == true)
                {
                    failureValue = rule.FailureValueFullName;
                }
                else
                {
                    failureValue = rule.FailureValue;
                }

                ruleOutput = ruleOutput + "\\t[][]" + expStr + "\\t[][]" + targetProperty + "\\t[][]" + successValue + "\\t[][]" + failureValue + "\\t[][]" + rule.Message;
                System.Diagnostics.Debug.WriteLine(ruleOutput);
            }
        }

        private void ProcessNode(ExpressionDesign exp, ref string expStr)
        {
            string logicalOperator = "";
            switch (exp.LogicalOperatorTypeId)
            {
                case 1:
                    logicalOperator = " AND ";
                    break;
                case 2:
                    logicalOperator = " OR ";
                    break;
                default:
                    logicalOperator = " NO LOGICAL OPERATOR ";
                    break;
            }
            if (exp.Expressions != null)
            {
                int count = exp.Expressions.Count;
                int idx = 0;
                foreach (var expr in exp.Expressions)
                {
                    idx++;
                    switch (expr.ExpressionTypeId)
                    {
                        case 1:
                            ProcessNode(expr, ref expStr);
                            break;
                        case 2:
                            ProcessLeaf(expr, ref expStr);
                            break;
                    }
                    if (idx < count)
                    {
                        expStr = expStr + logicalOperator;
                    }
                }
                expStr = " [ " + expStr + " ] ";
            }
        }

        private void ProcessLeaf(ExpressionDesign exp, ref string expStr)
        {
            string leftOperand = String.IsNullOrEmpty(exp.LeftOperandName) ? @"""" + exp.LeftOperand + @"""" : exp.LeftOperandName;
            string rightOperand = String.IsNullOrEmpty(exp.RightOperandName) ? @"""" + exp.RightOperand + @"""" : exp.RightOperandName;
            string operatorType = "";
            switch (exp.OperatorTypeId)
            {
                case 1:
                    operatorType = " EQUALS ";
                    break;
                case 2:
                    operatorType = " GREATER THAN ";
                    break;
                case 3:
                    operatorType = " LESS THAN ";
                    break;
                case 4:
                    operatorType = " CONTAINS ";
                    break;
                case 5:
                    operatorType = " NOT EQUALS ";
                    break;
                case 6:
                    operatorType = " GREATER THAN OR EQUAL TO ";
                    break;
                case 7:
                    operatorType = " LESS THAN OR EQUAL TO ";
                    break;
                case 8:
                    operatorType = " IS NULL";
                    break;
                default:
                    operatorType = " OPERATOR MISSING ";
                    break;
            }
            expStr = expStr + " ( " + leftOperand + operatorType + rightOperand + " ) ";
        }

        public bool RunSectionVisibleRule(string sectionName)
        {
            bool result = true;
            var sectionDesign = _detail.Sections.Where(s => s.GeneratedName == sectionName).FirstOrDefault();
            if (sectionDesign != null)
            {
                List<RuleDesign> sectionRules = _detail.Rules.Where(a => a.UIElementFullName == sectionName && a.TargetPropertyTypeId == 4).ToList();
                if (sectionRules.Count > 0)
                {
                    ConfigureRuleProcessor ruleProcessor = new ConfigureRuleProcessor(_detail, _formDataInstanceManager, _sectionName, _formInstanceId, sectionRules, _folderVersionServices, _formDesignServices, _formInstanceService, _ruleExecutionLogService, _ruleExecutionLogParentRowID);
                    result = ruleProcessor.RunRule(sectionRules[0]);
                }
            }
            return result;
        }
    }
}