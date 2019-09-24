using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.DocumentRule;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.dependencyresolution;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.ruleinterpreter;
using tmg.equinox.web.FormDesignManager;
using tmg.equinox.web.FormInstanceManager;
using tmg.equinox.web.FormInstanceProcessor;
using tmg.equinox.web.FormInstanceProcessor.ExpressionBuilder;
using tmg.equinox.web.sourcehandler;

namespace tmg.equinox.web.CollateralHelper
{
    public class SBDesignDataHelper
    {
        private int _tenantID = 1;
        private int _sbDesignID = 1272;
        private int _formDesignVersionID;
        private int _formInstanceID;
        private int _folderVersionID;
        private int? _userID;
        private string _currentUserName;
        private IUIElementService _uiElementService;
        private IFormDesignService _formDesignService;
        private IFolderVersionServices _folderVersionService;
        private IFormInstanceService _formInstanceService;
        private IFormInstanceDataServices _formInstanceDataService;
        private SourceHandlerDBManager _sourceHandlerdbManager;
        private FormInstanceDataManager _formDataInstanceManager;
        private IMasterListService _masterListService;
        private FormDesignVersionDetail _detail;

        public SBDesignDataHelper(int formInstanceID, int folderVersionID, int formDesignVersionID, int userID, string currentUserName,
                    IUIElementService uiElementService, IFormDesignService formDesignService, IFolderVersionServices folderVersionServices,
                    IFormInstanceService formInstanceService, IFormInstanceDataServices formInstanceDataServices, IMasterListService masterListService)
        {
            this._formInstanceID = formInstanceID;
            this._folderVersionID = folderVersionID;
            this._formDesignVersionID = formDesignVersionID;
            this._userID = userID;
            this._currentUserName = currentUserName;
            this._uiElementService = uiElementService;
            this._formDesignService = formDesignService;
            this._folderVersionService = folderVersionServices;
            this._formInstanceService = formInstanceService;
            this._formInstanceDataService = formInstanceDataServices;
            this._masterListService = masterListService;
            this._formDataInstanceManager = new FormInstanceDataManager(_tenantID, _userID, _formInstanceDataService, _currentUserName,_folderVersionService);
            this._sourceHandlerdbManager = new SourceHandlerDBManager(_tenantID, _folderVersionService, _formDataInstanceManager, _formDesignService, _masterListService);
            this._detail = this.GetFormDesignVersionDetail();
        }

        public string ProcessSBDesign(List<SBConfig> config)
        {
            SourceDesignDetails design = _formInstanceService.GetViewByAnchor(_folderVersionID, _sbDesignID, _formInstanceID); ;
            List<JToken> sourceDocumentRules = GetAnchorDependentRules(design.RuleEventTree, _detail.FormName);

            int baseFormInstanceID = config[0].FormInstanceID;
            int productIndex = 1;
            foreach (SBConfig item in config)
            {
                this.UpdateSBDesignJSON(design.FormInstanceId, productIndex, config.Count);

                CurrentRequestContext requestContext = new CurrentRequestContext();
                requestContext.RuleAliasesLoadedForSection.Add(SBDesignSettingConstant.RuleAlias, design.FormInstanceId);

                ExpressionRuleTreeProcessor ruleTreeProcessor = new ExpressionRuleTreeProcessor(item.FormInstanceID, _uiElementService, item.FolderVersionID, _sourceHandlerdbManager, _formDesignService, _formDataInstanceManager, _detail, _formInstanceService, _userID, requestContext);

                foreach (JToken sectionRule in sourceDocumentRules)
                {
                    List<JToken> sectionInnerRules = sectionRule.SelectToken(DocumentRuleConstant.InnerRules).ToList();
                    foreach (var rule in sectionInnerRules)
                    {
                        JToken ruleTree = GetRuleTree(rule, design.FormDesignVersionId);
                        ruleTreeProcessor.ProcessRuleTree(ruleTree, RuleEventType.DOCUMENTSAVE);
                    }
                }

                productIndex = productIndex + 1;
            }
            return GetFormInstanceData(design.FormInstanceId);
        }

        private void UpdateSBDesignJSON(int sbFormInstanceID, int index, int productCount)
        {
            FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(_tenantID, 1258, _formDesignService);
            FormDesignVersionDetail detail = formDesignVersionMgr.GetFormDesignVersion(true);

            string sectionData = _formDataInstanceManager.GetSectionData(sbFormInstanceID, SBDesignSettingConstant.SectionName, false, detail, false, false);
            if (string.IsNullOrEmpty(sectionData))
            {
                sectionData = "{\"ExpressionBuilderMultiplePlanHandler\":{\"Layout\":\"\",\"ProductNumber\":\"\"}}";
            }
            JObject expSettings = JObject.Parse(sectionData);
            expSettings[SBDesignSettingConstant.SectionName][SBDesignSettingConstant.ProductIndex] = Convert.ToString(index);
            expSettings[SBDesignSettingConstant.SectionName][SBDesignSettingConstant.ProductCount] = Convert.ToString(productCount);
            _formDataInstanceManager.SetCacheData(sbFormInstanceID, SBDesignSettingConstant.SectionName, expSettings.ToString());
        }

        private FormDesignVersionDetail GetFormDesignVersionDetail()
        {
            FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(_tenantID, _formDesignVersionID, _formDesignService);
            FormDesignVersionDetail detail = formDesignVersionMgr.GetFormDesignVersion(true);

            return detail;
        }

        private List<JToken> GetAnchorDependentRules(string eventRuleTree, string formName)
        {
            JArray documentRules = ((JArray)JObject.Parse(eventRuleTree)[SBDesignSettingConstant.SourceDocument]);
            JToken sourceRule = documentRules.ToList()
                                .Where(whr => whr[SBDesignSettingConstant.Document].ToString() == formName)
                                .FirstOrDefault()
                                .SelectToken(SBDesignSettingConstant.SourceSectionRule);
            return sourceRule.ToList();
        }

        private JToken GetRuleTree(JToken sectionRule, int formDesignVersionId)
        {
            int ruleId = Convert.ToInt32(sectionRule.SelectToken(DocumentRuleConstant.RuleId));
            ExpressionBuilderTreeReader treeBuilder = new ExpressionBuilderTreeReader(_tenantID, formDesignVersionId, _uiElementService, _formDesignService);
            JToken ruleTreeObject = treeBuilder.GetRuleTree(ruleId);
            return ruleTreeObject;
        }

        private string GetFormInstanceData(int formInstanceID)
        {
            string formData = _folderVersionService.GetFormInstanceData(_tenantID, formInstanceID);
            if (!string.IsNullOrEmpty(formData))
            {
                JObject formInstanceData = JObject.Parse(formData);
                List<JToken> sectionList = _formDataInstanceManager.GetSectionsFromCache(formInstanceID, Convert.ToInt32(_userID));
                foreach (JToken section in sectionList)
                {
                    string sectionName = section.ToString();
                    JObject sectionData = JObject.Parse(_formDataInstanceManager.GetSectionData(formInstanceID, sectionName, false, _detail, false, false));
                    formInstanceData[sectionName] = sectionData.SelectToken(sectionName);
                    _formDataInstanceManager.RemoveSectionsData(formInstanceID, sectionName, Convert.ToInt32(_userID));
                }
                _formDataInstanceManager.RemoveSectionListFromCache(formInstanceID, Convert.ToInt32(_userID));
                formData = JsonConvert.SerializeObject(formInstanceData);
            }
            return formData;
        }
    }
}