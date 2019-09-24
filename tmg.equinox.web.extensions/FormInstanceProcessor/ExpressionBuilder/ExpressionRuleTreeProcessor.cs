using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.web.FormInstanceManager;
using tmg.equinox.web.FormInstanceProcessor.SourceTargetDataManager.SourceHandler;
using tmg.equinox.web.sourcehandler;
using tmg.equinox.ruleinterpreter.pathhelper;
using tmg.equinox.ruleinterpreter.jsonhelper;
using Newtonsoft.Json;
using tmg.equinox.web.FormInstanceProcessor.ExpressionBuilder;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.web.FormDesignManager;
using tmg.equinox.ruleinterpreter;

namespace tmg.equinox.web.FormInstanceProcessor
{
    public class ExpressionRuleTreeProcessor
    {
        int _folderVersionId;
        SourceHandlerDBManager _sourceDBHandlerManager;
        IUIElementService _uiElementService;
        IFormDesignService _formDesignService;
        FormInstanceDataManager _formInstanceDataManager;
        FormDesignVersionDetail _detail;
        IFormInstanceService _formInstanceService;
        int _tenantId = 1;
        int _formInstanceId;
        int? _userId;
        CurrentRequestContext _requestContext;
        public ExpressionRuleTreeProcessor(int formInstanceId, IUIElementService uiElementService, int folderVersionId, SourceHandlerDBManager sourceDBManager, IFormDesignService formDesignService, FormInstanceDataManager formInstanceDataManager, FormDesignVersionDetail detail, IFormInstanceService formInstanceService, int? userId, CurrentRequestContext requestContext)
        {
            _folderVersionId = folderVersionId;
            _sourceDBHandlerManager = sourceDBManager;
            _uiElementService = uiElementService;
            _formDesignService = formDesignService;
            _formInstanceService = formInstanceService;
            _formInstanceId = formInstanceId;
            _formInstanceDataManager = formInstanceDataManager;
            _detail = detail;
            _userId = userId;
            _requestContext = requestContext;
        }

        public void ProcessRuleTree(JToken ruleTree, RuleEventType eventType)
        {
            int ruleId = Convert.ToInt32(ruleTree.SelectToken(DocumentRuleConstant.RuleId));
            CompiledDocumentRule compiledRule = GetCompiledRule(ruleId);
            try
            {
                //check if this is for multiple target instances
                //specifically checking for VBID View currently which will have multiple instances of the child View
                int targetFormInstanceId;
                bool isTargetMultipleInstance = IsMultipleInstanceTarget(compiledRule, eventType);
                if (isTargetMultipleInstance == true)
                {
                    ProcessRuleTreeForMultipleTargets(compiledRule, eventType);
                }
                else
                {
                    RuleExecution execution = new RuleExecution(_userId, compiledRule, _formInstanceId, _folderVersionId, _sourceDBHandlerManager, _detail, _formDesignService, _formInstanceService, _requestContext);
                    JToken ruleOutput = execution.ProcessRule();

                    switch (eventType)
                    {
                        case RuleEventType.SECTIONLOAD:
                            break;
                        case RuleEventType.SECTIONSAVE:
                            UpdateTargetSection(_formInstanceId, compiledRule.Target.TargetPath, ruleOutput, eventType);
                            break;
                        case RuleEventType.DOCUMENTSAVE:
                            targetFormInstanceId = GetTargetFormInstanceId(compiledRule.Target.TargetPath);
                            UpdateTargetSection(targetFormInstanceId, compiledRule.Target.TargetPath, ruleOutput, eventType);
                            break;
                        case RuleEventType.DOCUMENTLOAD:
                            break;
                        case RuleEventType.SELECTDIALOG:
                            break;
                        default:
                            break;
                    }

                }

                List<JToken> childRules = GetChildRules(ruleTree);
                foreach (JToken childRule in childRules)
                {
                    if (!childRule.IsNullOrEmpty())
                        ProcessRuleTree(childRule, eventType);
                }

            }
            catch (Exception ex)
            {
                string customMsg = "An error occurred while processing Rule '" + ruleId + "'" + "For Target Path '" + compiledRule.Target.TargetPath + "'";
                Exception customException = new Exception(customMsg, ex);
                ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
            }
        }

        public void ProcessRuleTreeForMultipleTargets(CompiledDocumentRule compiledRule, RuleEventType eventType)
        {
            RuleExecution execution = new RuleExecution(_userId, compiledRule, _formInstanceId, _folderVersionId, _sourceDBHandlerManager, _detail, _formDesignService, _formInstanceService, _requestContext);
            //get target form instances
            string ruleAlias = compiledRule.Target.TargetPath.GetRuleAlias();
            List<string> ruleAliases = new List<string>();
            ruleAliases.Add(ruleAlias);
            Dictionary<string, List<int>> ruleAliasFormInstances = execution.GetRuleAliasBasedFormInstanceForMultipleTargets(ruleAliases);
            if (ruleAliasFormInstances != null && ruleAliasFormInstances.Count > 0)
            {
                var ruleAliasFI = ruleAliasFormInstances[ruleAlias];
                if (ruleAliasFI != null && ruleAliasFI.Count > 0)
                {
                    foreach(var formInstanceId in ruleAliasFI)
                    {
                        JToken ruleOutput = execution.ProcessRule(formInstanceId);
                        switch (eventType)
                        {
                            case RuleEventType.SECTIONLOAD:
                                break;
                            case RuleEventType.SECTIONSAVE:
                                UpdateTargetSection(_formInstanceId, compiledRule.Target.TargetPath, ruleOutput, eventType);
                                break;
                            case RuleEventType.DOCUMENTSAVE:
                                UpdateTargetSection(formInstanceId, compiledRule.Target.TargetPath, ruleOutput, eventType);
                                break;
                            case RuleEventType.DOCUMENTLOAD:
                                break;
                            case RuleEventType.SELECTDIALOG:
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        public JToken ProcessRuleTest(int ruleId)
        {
            CompiledDocumentRule compiledRule = GetCompiledRule(ruleId);
            RuleExecution execution = new RuleExecution(_userId, compiledRule, _formInstanceId, _folderVersionId, _sourceDBHandlerManager, _detail, _formDesignService, _formInstanceService, _requestContext);
            return execution.ProcessRule();
        }

        public string ProcessCompiledRule(CompiledDocumentRule rule)
        {
            RuleExecution execution = new RuleExecution(_userId, rule, _formInstanceId, _folderVersionId, _sourceDBHandlerManager, _detail, _formDesignService, _formInstanceService, _requestContext);
            JToken targetData = execution.ProcessRule();
            UpdateTargetSection(_formInstanceId, rule.Target.TargetPath.TrimEnd('.'), targetData, RuleEventType.SECTIONSAVE);
            return rule.Target.TargetPath.TrimEnd('.');
        }
        public string ProcessCompiledRuleForODM(CompiledDocumentRule rule)
        {
            RuleExecution execution = new RuleExecution(_userId, rule, _formInstanceId, _folderVersionId, _sourceDBHandlerManager, _detail, _formDesignService, _formInstanceService, _requestContext);
            JToken targetData = execution.ProcessRule();
            string sectionData = GetUpdatedTargetSection(_formInstanceId, rule.Target.TargetPath.TrimEnd('.'), targetData, RuleEventType.SECTIONSAVE);
            return sectionData;
        }
        public void ProcessRuleTreeOnLoad(JToken ruleTree, RuleEventType eventType, string sectionName)
        {
            int ruleId = Convert.ToInt32(ruleTree.SelectToken(DocumentRuleConstant.RuleId));
            CompiledDocumentRule compiledRule = GetCompiledRule(ruleId);
            try
            {
                List<string> ruleAliases = compiledRule.SourceContainer.RuleSources.Select(sel => sel.SourcePath.Substring(0, sel.SourcePath.IndexOf('[')
                                         )).Distinct().ToList();

                string targetSectionName = compiledRule.Target.TargetPath.Split('[')[1].Split('.')[0];
                string targetDesignName = compiledRule.Target.TargetPath.Split('[')[0];
                if (sectionName == targetSectionName && targetDesignName == _detail.FormName)
                {
                    bool isMasterListhasSource = false;
                    RuleAliasResolver resolver = new RuleAliasResolver(_detail.FormName, _detail.FormDesignId, _formInstanceId, _folderVersionId, _formInstanceService, _formDesignService, _userId);
                    foreach (string name in ruleAliases)
                    {
                        if (_requestContext.RuleAliasesMasterListMaps.ContainsKey(name) == false)
                        {
                            isMasterListhasSource = resolver.IsMasterListFormDesign(name);
                            _requestContext.RuleAliasesMasterListMaps.Add(name, isMasterListhasSource);
                        }
                        else
                        {
                            isMasterListhasSource = _requestContext.RuleAliasesMasterListMaps[name];
                        }
                        if (isMasterListhasSource)
                        {
                            break;
                        }
                    }

                    //if (isMasterListhasSource)
                    {
                        RuleExecution execution = new RuleExecution(_userId, compiledRule, _formInstanceId, _folderVersionId, _sourceDBHandlerManager, _detail, _formDesignService, _formInstanceService, _requestContext);
                        JToken ruleOutput = execution.ProcessRule();

                        switch (eventType)
                        {
                            case RuleEventType.SECTIONLOAD:
                                UpdateTargetSection(_formInstanceId, compiledRule.Target.TargetPath, ruleOutput, eventType);
                                break;
                        }

                        List<JToken> childRules = GetChildRules(ruleTree);
                        foreach (JToken childRule in childRules)
                        {
                            if (!childRule.IsNullOrEmpty())
                                ProcessRuleTreeOnLoad(childRule, eventType, sectionName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string targetpath = string.Empty;
                if (compiledRule != null)
                    targetpath = compiledRule.Target.TargetPath;
                string customMsg = "An error occurred while processing Rule '" + ruleId + "'" + "For Target Path '" + targetpath + "'";
                Exception customException = new Exception(customMsg, ex);
                try
                {
                    ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
                }
                catch
                {

                }
            }
        }

        public CompiledDocumentRule GetCompiledRule(int documentRuleId)
        {
            return CompiledDocumentRuleReadOnlyObjectCache.GetCompiledDocumentRule(documentRuleId, _tenantId, _folderVersionId, _uiElementService, _formDesignService);
        }

        private List<JToken> GetChildRules(JToken ruleTree)
        {
            return ruleTree.SelectToken(DocumentRuleConstant.InnerRules).ToList();
        }

        private void UpdateTargetSection(int formInstanceId, string targetPath, JToken ruleOutput, RuleEventType eventType)
        {
            //if (!ruleOutput.IsNullOrEmpty())  //TODO: Need to set Default JSON if Empty() comes for Update.
            //{
            TargetSectionSyncManager targetSyncManager = new TargetSectionSyncManager(targetPath, formInstanceId, ruleOutput, _sourceDBHandlerManager, _requestContext);
            string updatedSectionData = targetSyncManager.GetUpdatedSection(_detail.JSONData, eventType);
            string sectionName = targetPath.GetSectionName();

            if (eventType == RuleEventType.SECTIONLOAD)
            {
                _detail.JSONData = updatedSectionData;
            }
            else
            {
                _sourceDBHandlerManager.UpdateProcessedRuleSection(formInstanceId, sectionName, updatedSectionData);
            }
            // }
        }

        private int GetTargetFormInstanceId(string targetPath)
        {
            int formInstanceId = 0;
            string ruleAlias = targetPath.Substring(0, targetPath.IndexOf('['));

            if (_requestContext.RuleAliasesLoadedForSection.ContainsKey(ruleAlias) == false)
            {
                RuleAliasResolver resolver = new RuleAliasResolver(_detail.FormName, _detail.FormDesignId, _formInstanceId, _folderVersionId, _formInstanceService, _formDesignService, _userId);
                formInstanceId = resolver.Resolve(ruleAlias);
                _requestContext.RuleAliasesLoadedForSection.Add(ruleAlias, formInstanceId);
            }
            else
            {
                formInstanceId = _requestContext.RuleAliasesLoadedForSection[ruleAlias];
            }

            return formInstanceId;
        }

        private string GetUpdatedTargetSection(int formInstanceId, string targetPath, JToken ruleOutput, RuleEventType eventType)
        {
            string updatedSectionData = string.Empty;
            try
            {
                TargetSectionSyncManager targetSyncManager = new TargetSectionSyncManager(targetPath, formInstanceId, ruleOutput, _sourceDBHandlerManager, _requestContext);
                updatedSectionData = targetSyncManager.GetUpdatedSection(_detail.JSONData, eventType);
            }
            catch (Exception wx)
            {
            }
            //if (!ruleOutput.IsNullOrEmpty())  //TODO: Need to set Default JSON if Empty() comes for Update.
            //{
            //TargetSectionSyncManager targetSyncManager = new TargetSectionSyncManager(targetPath, formInstanceId, ruleOutput, _sounceDBManager, _requestContext);
            //string updatedSectionData = targetSyncManager.GetUpdatedSection(_detail.JSONData, eventType);
            //string sectionName = targetPath.GetSectionName();

            //if (eventType == RuleEventType.SECTIONLOAD)
            //{
            //    _detail.JSONData = updatedSectionData;
            //}
            //else
            //{
            //    _sounceDBManager.UpdateProcessedRuleSection(formInstanceId, sectionName, updatedSectionData);
            //}
            // }
            return updatedSectionData;
        }

        private bool IsMultipleInstanceTarget(CompiledDocumentRule rule, RuleEventType eventType)
        {
            bool retVal = false;
            string ruleAlias = rule.Target.TargetPath.GetRuleAlias();
            if (ruleAlias.ToLower() == "vbidview" && eventType==RuleEventType.DOCUMENTSAVE)
            {
                retVal = true;
            }
            return retVal;
        }
    }
}