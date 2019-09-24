using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.DocumentRule;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.ruleinterpreter.pathhelper;
using tmg.equinox.ruleinterpreter.jsonhelper;
using tmg.equinox.ruleprocessor;
using tmg.equinox.caching;
using tmg.equinox.forminstanceprocessor.expressionbuilder;
using tmg.equinox.ruleinterpreter;
using tmg.equinox.infrastructure.util;

namespace tmg.equinox.expressionbuilder
{
    public class ExpressionBuilderRuleProcessor
    {
        private int _tenantId = 1;
        private int _formInstanceId;
        private int _folderVersionId;
        private int _formDesignVersionId;
        private bool _isFolderReleased;
        private FormDesignVersionDetail _detail;
        private IFolderVersionServices _folderVersionServices;
        private IUIElementService _uiElementService;
        private IFormDesignService _formDesignService;
        private IFormInstanceService _formInstanceService;
        private FormInstanceDataManager _formDataInstanceManager;
        private SourceHandlerDBManager _sourceHandlerdbManager;
        private FormInstanceSectionDataCacheHandler _sectionCachehandler;
        private int? _userId;
        string _sectionName;
        string _previousSectionData;
        string _currentSectionData;

        public ExpressionBuilderRuleProcessor(int? userId, int tenantId, int formInstanceId, int folderVersionId, FormDesignVersionDetail detail, int formDesignVersionId, bool isFolderReleased, IFolderVersionServices folderVersionServices, IUIElementService uiElementService, FormInstanceDataManager formDataInstanceManager, IFormDesignService formDesignService, string sectionName, string previousSectionData, string currentSectionData, IMasterListService masterListService, IFormInstanceService formInstanceService)
        {
            this._tenantId = tenantId;
            this._userId = userId;
            this._formInstanceId = formInstanceId;
            this._folderVersionId = folderVersionId;
            this._formDesignVersionId = formDesignVersionId;
            this._isFolderReleased = isFolderReleased;
            this._detail = detail;
            this._folderVersionServices = folderVersionServices;
            this._formInstanceService = formInstanceService;
            this._uiElementService = uiElementService;
            this._formDataInstanceManager = formDataInstanceManager;
            this._formDesignService = formDesignService;
            this._sectionName = sectionName;
            this._previousSectionData = previousSectionData;
            this._currentSectionData = currentSectionData;
            this._sourceHandlerdbManager = new SourceHandlerDBManager(_tenantId, _folderVersionServices, _formDataInstanceManager, _formDesignService, masterListService);
            this._sectionCachehandler = new FormInstanceSectionDataCacheHandler();
        }


        public void ProcessSectionSave(string section)
        {
            List<int> processedRuleIds = new List<int>();
            //get rule event map
            List<JToken> sectionRules = GetSourceSectionRules(section);
            CurrentRequestContext requestContext = new CurrentRequestContext();

            //process rule
            ExpressionRuleTreeProcessor ruleTreeProcessor = new ExpressionRuleTreeProcessor(_formInstanceId, _uiElementService, _folderVersionId, _sourceHandlerdbManager, _formDesignService, _formDataInstanceManager, _detail, _formInstanceService, _userId, requestContext);

            //get rule
            foreach (JObject sectionRule in sectionRules)
            {
                //get execution tree 
                JToken ruleTree = GetRuleTree(sectionRule, _formDesignVersionId);
                if (!ruleTree.IsNullOrEmpty())
                {
                    int ruleId = Convert.ToInt32(ruleTree.SelectToken(DocumentRuleConstant.RuleId));
                    if (!processedRuleIds.Contains(ruleId)) //Check if rule already processed
                    {
                        bool isDataChange = hasPartChange(ruleTree, ruleTreeProcessor);
                        if (isDataChange)
                        {
                            ruleTreeProcessor.ProcessRuleTree(ruleTree, RuleEventType.SECTIONSAVE);
                        }
                        processedRuleIds.Add(ruleId);
                    }
                }
            }
        }


        public void ProcessDocumentSave()
        {

            //Find the sources for current FormDesign
            List<SourceDesignDetails> sourceDesignVersions = _uiElementService.GetParentDesignDetails(_folderVersionId, _detail.FormDesignId, _formInstanceId);

            foreach (SourceDesignDetails sourceDesignVersion in sourceDesignVersions)
            {
                // Get Rules for ViewDesign based on EventTreeJSON string
                List<JToken> sourceDocumentRules = GetSourceDocumentRules(sourceDesignVersion.RuleEventTree, null);
                CurrentRequestContext requestContext = new CurrentRequestContext();
                ExpressionRuleTreeProcessor ruleTreeProcessor = new ExpressionRuleTreeProcessor(_formInstanceId, _uiElementService, _folderVersionId, _sourceHandlerdbManager, _formDesignService, _formDataInstanceManager, _detail, _formInstanceService, _userId, requestContext);

                //Iterate through Each section 
                foreach (JToken sectionRule in sourceDocumentRules)
                {
                    //get execution tree 
                    List<JToken> sectionInnerRules = sectionRule.SelectToken(DocumentRuleConstant.InnerRules).ToList();

                    //Process Each Rule for the Section
                    foreach (var rule in sectionInnerRules)
                    {
                        JToken ruleTree = GetRuleTree(rule, sourceDesignVersion.FormDesignVersionId);
                        ruleTreeProcessor.ProcessRuleTree(ruleTree, RuleEventType.DOCUMENTSAVE);
                    }
                }
            }
        }

        public void ProcessViewDocumentRules()
        {

            //Find the sources for current FormDesign
            List<SourceDesignDetails> sourceDesignVersions = _uiElementService.GetParentDesignDetails(_folderVersionId, _detail.FormDesignId, _formInstanceId);

            foreach (SourceDesignDetails sourceDesignVersion in sourceDesignVersions)
            {
                // Get Rules for ViewDesign based on EventTreeJSON string
                CurrentRequestContext requestContext = new CurrentRequestContext();
                ExpressionRuleTreeProcessor ruleTreeProcessor = new ExpressionRuleTreeProcessor(_formInstanceId, _uiElementService, _folderVersionId, _sourceHandlerdbManager, _formDesignService, _formDataInstanceManager, _detail, _formInstanceService, _userId, requestContext);
                ExpressionBuilderEventMapReader eventMapReader = new ExpressionBuilderEventMapReader(_tenantId, sourceDesignVersion.FormDesignVersionId, _uiElementService, _formDesignService);
                List<JToken> documentRules = eventMapReader.GetDocumentRules();
                //Iterate through Each section 
                foreach (JToken sectionRule in documentRules)
                {
                    //get execution tree 
                    List<JToken> sectionInnerRules = sectionRule.SelectToken(DocumentRuleConstant.InnerRules).ToList();

                    //Process Each Rule for the Section
                    foreach (var rule in sectionInnerRules)
                    {
                        JToken ruleTree = GetRuleTree(rule, sourceDesignVersion.FormDesignVersionId);
                        ruleTreeProcessor.ProcessRuleTree(ruleTree, RuleEventType.DOCUMENTSAVE);
                    }
                }
            }
        }


        public void ProcessCollateralViewDocumentRules()
        {
            //Find the sources for current FormDesign
            SourceDesignDetails sourceDesignVersion = _uiElementService.GetParentDesignDetails(_folderVersionId, _detail.FormDesignId, _formInstanceId)
                                                               .Where(x => x.FormDesignVersionId == _formDesignVersionId).FirstOrDefault();

            // Get Rules for ViewDesign based on EventTreeJSON string
            CurrentRequestContext requestContext = new CurrentRequestContext();
            ExpressionRuleTreeProcessor ruleTreeProcessor = new ExpressionRuleTreeProcessor(_formInstanceId, _uiElementService, _folderVersionId, _sourceHandlerdbManager, _formDesignService, _formDataInstanceManager, _detail, _formInstanceService, _userId, requestContext);
            ExpressionBuilderEventMapReader eventMapReader = new ExpressionBuilderEventMapReader(_tenantId, sourceDesignVersion.FormDesignVersionId, _uiElementService, _formDesignService);
            List<JToken> documentRules = eventMapReader.GetDocumentRules();

            //Iterate through Each section 
            foreach (JToken sectionRule in documentRules)
            {
                //get execution tree 
                List<JToken> sectionInnerRules = sectionRule.SelectToken(DocumentRuleConstant.InnerRules).ToList();

                //Process Each Rule for the Section
                foreach (var rule in sectionInnerRules)
                {
                    JToken ruleTree = GetRuleTree(rule, sourceDesignVersion.FormDesignVersionId);
                    ruleTreeProcessor.ProcessRuleTree(ruleTree, RuleEventType.DOCUMENTSAVE);
                }
            }

        }

        public void ProcessSectionLoad(string section)
        {
            List<int> processedRuleIds = new List<int>();
            //get rule event map
            List<JToken> sectionRules = string.IsNullOrEmpty(section) ? GetTargetSectionRules() : GetTargetSectionRules(section);

            CurrentRequestContext requestContext = new CurrentRequestContext();
            //process rule
            ExpressionRuleTreeProcessor ruleTreeProcessor = new ExpressionRuleTreeProcessor(_formInstanceId, _uiElementService, _folderVersionId, _sourceHandlerdbManager, _formDesignService, _formDataInstanceManager, _detail, _formInstanceService, _userId, requestContext);
            //get rule
            foreach (JObject sectionRule in sectionRules)
            {
                //get execution tree 
                JToken ruleTree = GetRuleTree(sectionRule, _formDesignVersionId);
                if (!ruleTree.IsNullOrEmpty())
                {
                    int ruleId = Convert.ToInt32(ruleTree.SelectToken(DocumentRuleConstant.RuleId));
                    if (!processedRuleIds.Contains(ruleId)) //Check if rule already processed
                    {

                        // CompiledDocumentRule compiledRule = GetCompiledRule(ruleId);
                        //bool isDataChange = hasPartChange(ruleTree, ruleTreeProcessor);
                        //if (isDataChange)
                        //{
                        ruleTreeProcessor.ProcessRuleTreeOnLoad(ruleTree, RuleEventType.SECTIONLOAD, _sectionName);
                        //}
                        processedRuleIds.Add(ruleId);
                    }
                }
            }
        }

        private JToken GetRuleTree(JToken sectionRule, int formDesignVersionId)
        {
            int ruleId = Convert.ToInt32(sectionRule.SelectToken(DocumentRuleConstant.RuleId));
            ExpressionBuilderTreeReader treeBuilder = new ExpressionBuilderTreeReader(_tenantId, formDesignVersionId, _uiElementService, _formDesignService);
            JToken ruleTreeObject = treeBuilder.GetRuleTree(ruleId);
            return ruleTreeObject;
        }

        //GetSectionRules
        private List<JToken> GetSourceSectionRules(string section)
        {
            ExpressionBuilderEventMapReader eventMapReader = new ExpressionBuilderEventMapReader(_tenantId, _formDesignVersionId, _uiElementService, _formDesignService);
            List<JToken> sectionRules = eventMapReader.GetSourceSectionRules(section);
            return sectionRules;
        }

        private bool hasPartChange(JToken ruleTree, ExpressionRuleTreeProcessor ruleTreeProcessor)
        {
            bool isDataChange = false;
            List<RuleSourceItem> sourceList = new List<RuleSourceItem>();
            int ruleId = Convert.ToInt32(ruleTree.SelectToken(DocumentRuleConstant.RuleId));
            CompiledDocumentRule compiledRule = ruleTreeProcessor.GetCompiledRule(ruleId);
            if (compiledRule != null)
            {
                sourceList = compiledRule.SourceContainer.RuleSources.Where(a => a.SourcePath.GetSectionName() == _sectionName).ToList();

                SectionSaveHashCheckOptimizer hashOptimizer = new SectionSaveHashCheckOptimizer(_previousSectionData, _currentSectionData);
                foreach (RuleSourceItem item in sourceList)
                {
                    List<string> path = item.SourcePath.GetElementPaths();
                    isDataChange = hashOptimizer.hasPartChanged(path[0]);
                    if (isDataChange)
                    {
                        break;
                    }
                }
            }
            return isDataChange;
        }

        //GetSourceDocumentRules
        private List<JToken> GetSourceDocumentRules(string ruleEventString, List<string> updatedSections)
        {
            //Get Visted Section for the Folder
            List<string> visitedSections = updatedSections ?? _sectionCachehandler.GetSectionListFromCache(_formInstanceId, _userId).Select(sel => (string)sel).ToList();

            List<JToken> documentRules = new List<JToken>();

            if (ruleEventString != null && ruleEventString != "")
            {
                //Parse ruleEventString to JObject
                JObject sourceDocumentRules = JObject.Parse(ruleEventString);

                //GetSourceDocumetRules
                JArray sourceDocumentEventTree = (JArray)sourceDocumentRules[DocumentRuleConstant.Sourcedocuments];

                //GetDocumentRuleForCurrentForm
                if (sourceDocumentEventTree.Count != 0)
                {
                    if (sourceDocumentEventTree.ToList().Where(whr => whr[DocumentRuleConstant.document].ToString() == _detail.FormName)
                                                            .FirstOrDefault() != null)
                    {
                        JToken sectionRules = sourceDocumentEventTree.ToList().Where(whr => whr[DocumentRuleConstant.document].ToString() == _detail.FormName)
                                          .FirstOrDefault().SelectToken(DocumentRuleConstant.sourceSectionRules);

                        //Get Section Rules for Only Visited Section
                        documentRules = sectionRules.ToList().Where(whr => visitedSections.Contains(whr[DocumentRuleConstant.section].ToString())).Select(sel => sel).ToList();
                    }
                }
            }
            return documentRules;
        }


        private List<JToken> GetTargetSectionRules(string section)
        {
            ExpressionBuilderEventMapReader eventMapReader = new ExpressionBuilderEventMapReader(_tenantId, _formDesignVersionId, _uiElementService, _formDesignService);
            List<JToken> sectionRules = eventMapReader.GetTargetSectionRules(section);
            return sectionRules;
        }

        private List<JToken> GetTargetSectionRules()
        {
            ExpressionBuilderEventMapReader eventMapReader = new ExpressionBuilderEventMapReader(_tenantId, _formDesignVersionId, _uiElementService, _formDesignService);
            List<JToken> sectionRules = eventMapReader.GetTargetSectionRules();
            return sectionRules;
        }
    }
}