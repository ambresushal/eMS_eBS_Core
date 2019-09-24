using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.DocumentRule;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.web.DataSource;
using tmg.equinox.web.FormInstanceManager;
using tmg.equinox.web.FormInstanceProcessor.ExpressionBuilder;
using tmg.equinox.web.FormInstanceProcessor.SourceTargetDataManager.SourceHandler;
using tmg.equinox.web.Framework.Caching;
using tmg.equinox.web.sourcehandler;
using tmg.equinox.ruleinterpreter.pathhelper;
using tmg.equinox.ruleinterpreter.jsonhelper;
using Newtonsoft.Json;
using tmg.equinox.rules.oongroups;
using tmg.equinox.infrastructure.util;
using tmg.equinox.web.FormDesignManager;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.documentcomparer.RepeaterCompareUtils;
using tmg.equinox.ruleinterpreter;
//using tmg.equinox.mlcascade.documentcomparer.RepeaterCompareUtils;

namespace tmg.equinox.web.FormInstanceProcessor
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
        private IDataCacheHandler _cacheHandler;
        private int? _userId;
        string _sectionName;
        string _previousSectionData;
        string _currentSectionData;
        Dictionary<int, Dictionary<string, dynamic>> _expressionRuleActivityLog = new Dictionary<int, Dictionary<string, dynamic>>();
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
            this._cacheHandler = DataCacheHandlerFactory.GetHandler();
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


        public void ProcessDocumentSave(List<string> visitedSections)
        {

            //Find the sources for current FormDesign
            List<SourceDesignDetails> sourceDesignVersions = _uiElementService.GetParentDesignDetails(_folderVersionId, _detail.FormDesignId, _formInstanceId);

            foreach (SourceDesignDetails sourceDesignVersion in sourceDesignVersions)
            {
                // Get Rules for ViewDesign based on EventTreeJSON string
                List<JToken> sourceDocumentRules = GetSourceDocumentRules(sourceDesignVersion.RuleEventTree, visitedSections);
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
                _expressionRuleActivityLog.Add(sourceDesignVersion.FormInstanceId, requestContext.ExpressionRuleActivityLog);
            }
            //_formDataInstanceManager.SaveTargetSectionsData(_formInstanceId, _folderVersionServices, _formDesignService);
            if (sourceDesignVersions.Count > 0)
            {
                ProcessOONGroups(sourceDesignVersions.Where(s => s.FormName == "PBPView").FirstOrDefault());
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
                    try
                    {
                        JToken ruleTree = GetRuleTree(rule, sourceDesignVersion.FormDesignVersionId);
                        ruleTreeProcessor.ProcessRuleTree(ruleTree, RuleEventType.DOCUMENTSAVE);
                    }
                    catch (Exception ex)
                    {
                        string customMsg = "An error occurred while processing Rule -";
                        Exception customException = new Exception(customMsg, ex);
                        ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
                    }
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

        public void SaveExpressionRuleActivityLogData(string currentUserName)
        {
            int folderId = 0; int formDesignId = 0;
            try
            {
                if (_expressionRuleActivityLog != null && _expressionRuleActivityLog.Count > 0)
                {
                    FolderVersionViewModel fldrVersion = _folderVersionServices.GetFolderVersionById(_folderVersionId);
                    if (fldrVersion != null)
                    {
                        folderId = fldrVersion.FolderId;
                    }
                    //Find the sources for current FormDesign
                    List<SourceDesignDetails> sourceDesignVersions = _uiElementService.GetParentDesignDetails(_folderVersionId, _detail.FormDesignId, _formInstanceId);
                    foreach (SourceDesignDetails sourceDesignVersion in sourceDesignVersions)
                    {
                        FormDesignVersionRowModel model = _formDesignService.GetFormDesignVersionById(sourceDesignVersion.FormDesignVersionId);
                        if (model != null)
                        {
                            formDesignId = model.FormDesignId.Value;
                        }
                        if (_expressionRuleActivityLog.ContainsKey(sourceDesignVersion.FormInstanceId) == true)
                        {
                            Dictionary<string, dynamic> activityHistory = _expressionRuleActivityLog[sourceDesignVersion.FormInstanceId];
                            if (activityHistory != null && activityHistory.Count > 0)
                            {
                                FormDesignVersionDetail detail = FormDesignVersionDetailReadOnlyObjectCache.GetFormDesignVersionDetail(_tenantId, sourceDesignVersion.FormDesignVersionId, _formDesignService);
                                if (detail != null)
                                {
                                    List<ActivityLogModel> activityLogs = GetActivityLogModel(sourceDesignVersion.FormInstanceId, activityHistory, detail, currentUserName);
                                    if (activityLogs != null && activityLogs.Count > 0)
                                    {
                                        _folderVersionServices.SaveFormInstanceAvtivitylogData(_tenantId, sourceDesignVersion.FormInstanceId, folderId, _folderVersionId, formDesignId, sourceDesignVersion.FormDesignVersionId, activityLogs);
                                    }
                                }
                            }
                        }
                    }
                    _expressionRuleActivityLog = new Dictionary<int, Dictionary<string, dynamic>>();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
        }
        private List<ActivityLogModel> GetActivityLogModel(int formInstanceId, Dictionary<string, dynamic> activityHistory, FormDesignVersionDetail detail, string currentUserName)
        {
            List<ActivityLogModel> activityLogData = new List<ActivityLogModel>();
            try
            {
                ActivityLogModel model = new ActivityLogModel();

                foreach (var entry in activityHistory)
                {
                    string elementType = String.Empty;
                    List<ElementDesign> repeaterElement = new List<ElementDesign>();
                    List<string> elementPath = GetElementPathLabel(entry.Key, detail, out elementType, out repeaterElement);
                    int elementPathLen = elementPath.Count;
                    dynamic dataObj = entry.Value;
                    if (!String.IsNullOrEmpty(elementType) && !elementType.Equals("repeater"))
                    {
                        model = new ActivityLogModel();
                        string activityDescription = "Value of {0} is changed from {1} to {2}.";
                        model.Description = String.Format(activityDescription, elementPath[elementPathLen - 1], dataObj.OldValue, dataObj.NewValue);
                        model.ElementPath = GetElementPath(elementPath);
                        model.Field = elementPath[elementPathLen - 1];
                        model.FolderVersionName = "";
                        model.FormInstanceID = formInstanceId;
                        model.IsNewRecord = true;
                        model.SubSectionName = "";
                        model.UpdatedBy = currentUserName;
                        model.RowNum = "";
                        model.UpdatedLast = DateTime.Now;
                        activityLogData.Add(model);
                    }
                    else if (!String.IsNullOrEmpty(elementType) && elementType.Equals("repeater"))
                    {
                        GetRepeaterActivityLog(dataObj.OldValue, dataObj.NewValue, repeaterElement, elementPath, formInstanceId, currentUserName, ref activityLogData);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return activityLogData;
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

        private void ProcessOONGroups(SourceDesignDetails sourceDesignVersion)
        {
            try
            {
                if (sourceDesignVersion != null && sourceDesignVersion.FormDesignId == 2367)
                {
                    FormDesignVersionDetail targetDetail = FormDesignVersionDetailReadOnlyObjectCache.GetFormDesignVersionDetail(_tenantId, sourceDesignVersion.FormDesignVersionId, _formDesignService);
                    string targetSectionOONGroups = _formDataInstanceManager.GetSectionData(sourceDesignVersion.FormInstanceId, "OONGroups", false, targetDetail, false, false);
                    string targetSectionOONGroupNumbers = _formDataInstanceManager.GetSectionData(sourceDesignVersion.FormInstanceId, "OONNumberofGroups", false, targetDetail, false, false);
                    FormDesignVersionDetail sourceDetail = FormDesignVersionDetailReadOnlyObjectCache.GetFormDesignVersionDetail(_tenantId, _formDesignVersionId, _formDesignService);
                    string sotData = _cacheHandler.Get(_tenantId, _formInstanceId, false, sourceDetail, _folderVersionServices, _userId);
                    JObject formInstanceData = JObject.Parse(sotData);
                    List<JToken> sections = _formDataInstanceManager.GetSectionsFromCache(_formInstanceId, _userId.Value);
                    foreach (JToken section in sections)
                    {
                        string sectionName = section.ToString();
                        JObject sectionData = JObject.Parse(_formDataInstanceManager.GetSectionData(_formInstanceId, sectionName, false, _detail, false, false));
                        formInstanceData[sectionName] = sectionData.SelectToken(sectionName);
                    }
                    sotData = JsonConvert.SerializeObject(formInstanceData);
                    OONGroupGenerator groupGen = new OONGroupGenerator(sotData, targetSectionOONGroups, targetSectionOONGroupNumbers, _formInstanceService, _formDesignVersionId);
                    string oonGroupSectionData = groupGen.GetOONGroups();
                    string oonGroupNumberSectionData = groupGen.GetOONGroupNumbers();
                    JToken groups = JToken.Parse(oonGroupSectionData);
                    var overrideToken = groups.SelectToken("OONGroups.ManualOverride");
                    bool generate = true;
                    if (overrideToken != null)
                    {
                        string manualOverride = overrideToken.ToString();
                        if (manualOverride.ToLower() == "true")
                        {
                            generate = false;
                        }
                    }
                    if (generate == true)
                    {
                        FormInstanceSectionDataCacheHandler handler = new FormInstanceSectionDataCacheHandler();
                        handler.AddTargetFormInstanceIdToCache(_formInstanceId, sourceDesignVersion.FormInstanceId, _userId);
                        handler.AddSectionData(sourceDesignVersion.FormInstanceId, "OONGroups", oonGroupSectionData, _userId);
                        handler.AddSectionData(sourceDesignVersion.FormInstanceId, "OONNumberofGroups", oonGroupNumberSectionData, _userId);
                    }
                }
            }
            catch (Exception ex)
            {
                string customMsg = "An error occurred while processing OON Groups for " + _formInstanceId;
                Exception customException = new Exception(customMsg, ex);
                ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
            }
        }
        private List<string> GetElementPathLabel(string targetPath, FormDesignVersionDetail detail, out string elementType, out List<ElementDesign> repeaterElement)
        {
            List<string> elementPath = new List<string>();
            repeaterElement = new List<ElementDesign>();
            elementType = string.Empty;
            try
            {
                if (!String.IsNullOrEmpty(targetPath))
                {
                    string[] pathArray = targetPath.Split('.');
                    int pathLength = pathArray.Length;
                    SectionDesign section = new SectionDesign();
                    ElementDesign element = new ElementDesign();
                    for (int z = 0; z < pathLength; z++)
                    {
                        if (z == 0)
                        {
                            section = GetSectionDesign(pathArray[z], detail);
                            if (section != null && !String.IsNullOrEmpty(section.Label))
                            {
                                elementPath.Add(section.Label);
                            }
                        }
                        else
                        {
                            if (z != pathLength - 1)
                            {
                                section = GetInnerSectionDesign(pathArray[z], section);
                                if (section != null && !String.IsNullOrEmpty(section.Label))
                                {
                                    elementPath.Add(section.Label);
                                }
                            }
                            else
                            {
                                element = GetElementDesign(section, pathArray[z]);
                                if (element != null && !String.IsNullOrEmpty(element.Label))
                                {
                                    elementPath.Add(element.Label);
                                    elementType = element.Type;
                                    if (elementType == "repeater")
                                    {
                                        repeaterElement = GetRepeaterElement(element);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return elementPath;
        }
        private List<ElementDesign> GetRepeaterElement(ElementDesign element)
        {
            List<ElementDesign> repeaterElements = new List<ElementDesign>();
            try
            {
                if (element != null && element.Repeater != null)
                {
                    repeaterElements = element.Repeater.Elements;
                }
            }
            catch (Exception ex)
            {
                repeaterElements = new List<ElementDesign>();
            }
            return repeaterElements;
        }
        private SectionDesign GetSectionDesign(string sectionName, FormDesignVersionDetail detail)
        {
            SectionDesign sectionDesign = detail.Sections.Where(a => a.GeneratedName == sectionName).FirstOrDefault();
            return sectionDesign;
        }
        private SectionDesign GetInnerSectionDesign(string sectionName, SectionDesign section)
        {
            SectionDesign sectionDesign = null;
            try
            {
                if (section != null)
                {
                    List<ElementDesign> elements = section.Elements;
                    if (elements != null && elements.Count > 0)
                    {
                        ElementDesign ds = elements.Where(a => a.GeneratedName == sectionName).FirstOrDefault();
                        if (ds != null && ds.Section != null)
                        {
                            sectionDesign = ds.Section;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return sectionDesign;
        }
        private ElementDesign GetElementDesign(SectionDesign section, string elementName)
        {
            ElementDesign elementDesign = null;
            if (section != null)
            {
                List<ElementDesign> elements = section.Elements;
                if (elements != null && elements.Count > 0)
                {
                    ElementDesign ds = elements.Where(a => a.GeneratedName == elementName).FirstOrDefault();
                    if (ds != null)
                    {
                        elementDesign = ds;
                    }
                }
            }
            return elementDesign;
        }
        private string GetElementPath(List<string> targetPath)
        {
            string elementPath = String.Empty;
            try
            {
                if (targetPath != null && targetPath.Count > 0)
                {
                    int pathLength = targetPath.Count;
                    for (int x = 0; x < pathLength; x++)
                    {
                        if (x != pathLength - 1)
                        {
                            elementPath += targetPath[x] + " => ";
                        }
                    }
                    elementPath = elementPath.Substring(0, elementPath.Length - 4);
                }
            }
            catch (Exception ex)
            {
            }
            return elementPath;
        }
        private string GetRepeaterElementPath(List<string> targetPath)
        {
            string elementPath = String.Empty;
            try
            {
                if (targetPath != null && targetPath.Count > 0)
                {
                    int pathLength = targetPath.Count;
                    for (int x = 0; x < pathLength; x++)
                    {
                        //if (x != pathLength - 1)
                        {
                            elementPath += targetPath[x] + " => ";
                        }
                    }
                    elementPath = elementPath.Substring(0, elementPath.Length - 4);
                }
            }
            catch (Exception ex)
            {
            }
            return elementPath;
        }
        private void GetRepeaterActivityLog(dynamic previousTokenValue, dynamic newTokenValue, List<ElementDesign> repeaterElement, List<string> elementPath, int formInstanceId, string currentUserName, ref List<ActivityLogModel> activityLogData)
        {
            List<JToken> processedRows = new List<JToken>();
            List<string> columnList = new List<string>();
            List<string> keyList = new List<string>();
            try
            {
                if (repeaterElement != null && repeaterElement.Count > 0)
                {
                    columnList = repeaterElement.Select(x => x.GeneratedName).ToList();
                    keyList = repeaterElement.Where(a => a.IsKey == true).Select(x => x.GeneratedName).ToList();
                }
                JToken previousTokenVal = JToken.FromObject(previousTokenValue);
                JToken newTokenVal = JToken.FromObject(newTokenValue);
                // Step:1 Get Rows that are in previousToken not in newToken(i.e Deleted after expression rule)
                var deletedRows = previousTokenVal.Except(newTokenVal, new RepeaterEqualityComparer(columnList));
                if (deletedRows != null && deletedRows.Count() > 0)
                {
                    processedRows.AddRange(deletedRows.ToList());
                    AddRepeaterActivityLog(keyList, elementPath, formInstanceId, currentUserName, deletedRows, "Delete", ref activityLogData);
                }
                // Step:2 Get Rows that are in newToken not in previousToken (i.e Added after expression rule)
                var addedRows = newTokenVal.Except(previousTokenVal, new RepeaterEqualityComparer(columnList));
                if (addedRows != null && addedRows.Count() > 0)
                {
                    processedRows.AddRange(addedRows.ToList());
                    AddRepeaterActivityLog(keyList, elementPath, formInstanceId, currentUserName, addedRows, "Add", ref activityLogData);
                }
                // Step:3 Get Rows that are in both newToken and previousToken (i.e No Changes)
                var unmodifiedRows = newTokenVal.Intersect(previousTokenVal, new RepeaterEqualityComparer(columnList));
                if (unmodifiedRows != null && unmodifiedRows.Count() > 0)
                {
                    processedRows.AddRange(unmodifiedRows.ToList());
                }
                // Step:4 Get Rows that are modified
                var modifiedRows = newTokenVal.Except(processedRows, new RepeaterEqualityComparer(columnList));
                if (modifiedRows != null && modifiedRows.Count() > 0)
                {
                    AddRepeaterActivityLog(keyList, elementPath, formInstanceId, currentUserName, modifiedRows, "Update", ref activityLogData);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void AddRepeaterActivityLog(List<string> keyList, List<string> elementPath, int formInstanceId, string currentUserName, IEnumerable<JToken> rowsUpdated, string operation, ref List<ActivityLogModel> activityLogData)
        {
            try
            {
                if (!String.IsNullOrEmpty(operation))
                {
                    string activityDescription = String.Empty;
                    switch (operation)
                    {
                        case "Delete":
                            activityDescription = "{0} Row Was Deleted";
                            break;
                        case "Add":
                            activityDescription = "{0} Row Was Added";
                            break;
                        case "Update":
                            activityDescription = "{0} Was Updated";
                            break;
                    }
                    string repeaterName = elementPath[elementPath.Count - 1];
                    foreach (var item in rowsUpdated)
                    {
                        ActivityLogModel model = new ActivityLogModel();
                        string keyVal = String.Empty;
                        model.Description = String.Format(activityDescription, repeaterName);
                        model.Field = String.Empty;
                        model.RowNum = GetRepeaterRowKeyValue(item, keyList);
                        model.ElementPath = GetElementPath(elementPath);
                        model.FolderVersionName = "";
                        model.FormInstanceID = formInstanceId;
                        model.IsNewRecord = true;
                        model.SubSectionName = "";
                        model.UpdatedBy = currentUserName;
                        model.UpdatedLast = DateTime.Now;
                        activityLogData.Add(model);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        private string GetRepeaterRowKeyValue(JToken rowUpdated, List<string> keyList)
        {
            string keyVal = String.Empty;
            try
            {
                if (rowUpdated != null && keyList != null)
                {
                    foreach (string key in keyList)
                    {
                        keyVal += (rowUpdated[key] ?? String.Empty) + "#";
                    }
                    keyVal = keyVal.Substring(0, keyVal.Length - 1);
                }
            }
            catch (Exception ex)
            {
            }
            return keyVal;
        }
    }
}