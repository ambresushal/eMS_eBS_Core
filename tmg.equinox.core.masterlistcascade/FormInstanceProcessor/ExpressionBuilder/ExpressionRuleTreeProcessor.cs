using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.ruleinterpreter.pathhelper;
using tmg.equinox.ruleinterpreter.jsonhelper;
using tmg.equinox.ruleprocessor;
using tmg.equinox.forminstanceprocessor.expressionbuilder;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.ruleinterpreter;

namespace tmg.equinox.expressionbuilder
{
    public class ExpressionRuleTreeProcessor
    {
        int _folderVersionId;
        SourceHandlerDBManager _sounceDBManager;
        IUIElementService _uiElementService;
        IFormDesignService _formDesignService;
        FormInstanceDataManager _formInstanceDataManager;
        FormDesignVersionDetail _detail;
        IFormInstanceService _formInstanceService;
        int _tenantId = 1;
        int _formInstanceId;
        int? _userId;
        CurrentRequestContext _requestContext;
        public ExpressionRuleTreeProcessor(int formInstanceId, IUIElementService uiElementService, int folderVersionId, SourceHandlerDBManager sounceDBManager, IFormDesignService formDesignService, FormInstanceDataManager formInstanceDataManager, FormDesignVersionDetail detail, IFormInstanceService formInstanceService, int? userId, CurrentRequestContext requestContext)
        {
            _folderVersionId = folderVersionId;
            _sounceDBManager = sounceDBManager;
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
                int targetFormInatnceId;

                RuleExecution execution = new RuleExecution(_userId, compiledRule, _formInstanceId, _folderVersionId, _sounceDBManager, _detail, _formDesignService, _formInstanceService, _requestContext);
                JToken ruleOutput = execution.ProcessRule();

                switch (eventType)
                {
                    case RuleEventType.SECTIONLOAD:
                        break;
                    case RuleEventType.SECTIONSAVE:
                        UpdateTargetSection(_formInstanceId, compiledRule.Target.TargetPath, ruleOutput, eventType);
                        break;
                    case RuleEventType.DOCUMENTSAVE:
                        targetFormInatnceId = GetTargetFormInstanceId(compiledRule.Target.TargetPath);
                        UpdateTargetSection(targetFormInatnceId, compiledRule.Target.TargetPath, ruleOutput, eventType);
                        break;
                    case RuleEventType.DOCUMENTLOAD:
                        break;
                    case RuleEventType.SELECTDIALOG:
                        break;
                    default:
                        break;
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

        public JToken ProcessRuleTest(int ruleId)
        {
            CompiledDocumentRule compiledRule = GetCompiledRule(ruleId);
            RuleExecution execution = new RuleExecution(_userId, compiledRule, _formInstanceId, _folderVersionId, _sounceDBManager, _detail, _formDesignService, _formInstanceService, _requestContext);
            return execution.ProcessRule();
        }


        public string ProcessCompiledRule(CompiledDocumentRule rule)
        {
            RuleExecution execution = new RuleExecution(_userId, rule, _formInstanceId, _folderVersionId, _sounceDBManager, _detail, _formDesignService, _formInstanceService, _requestContext);
            UpdateTargetSection(_formInstanceId, rule.Target.TargetPath.TrimEnd('.'), execution.ProcessRule(),RuleEventType.SECTIONSAVE);
            return rule.Target.TargetPath.TrimEnd('.');
        }

        public void ProcessRuleTreeOnLoad(JToken ruleTree, RuleEventType eventType, string sectionName)
        {
            int ruleId = Convert.ToInt32(ruleTree.SelectToken(DocumentRuleConstant.RuleId));
            CompiledDocumentRule compiledRule = GetCompiledRule(ruleId);
            try
            {
                List<string> ruleAlises = compiledRule.SourceContainer.RuleSources.Select(sel => sel.SourcePath.Substring(0, sel.SourcePath.IndexOf('[')
                                         )).Distinct().ToList();

                string targetSectionName = compiledRule.Target.TargetPath.Split('[')[1].Split('.')[0];
                string targetDesignName = compiledRule.Target.TargetPath.Split('[')[0];
                if (sectionName == targetSectionName && targetDesignName == _detail.FormName)
                {
                    bool isMasterListhasSource = false;
                    RuleAliasResolver resolver = new RuleAliasResolver(_detail.FormName, _formInstanceId, _folderVersionId, _formInstanceService, _formDesignService, _userId);
                    foreach (string name in ruleAlises)
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
                        RuleExecution execution = new RuleExecution(_userId, compiledRule, _formInstanceId, _folderVersionId, _sounceDBManager, _detail, _formDesignService, _formInstanceService, _requestContext);
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
                string customMsg = "An error occurred while processing Rule '" + ruleId + "'" + "For Target Path '" + compiledRule.Target.TargetPath + "'";
                Exception customException = new Exception(customMsg, ex);
                ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
            }
        }

        public CompiledDocumentRule GetCompiledRule(int documentRuleId)
        {
            ExpressionBuilderEventMapReader expressionEventMapper = new ExpressionBuilderEventMapReader(_tenantId, _folderVersionId, _uiElementService, _formDesignService);
            CompiledDocumentRule compiledDocument = expressionEventMapper.GetCompiledRule(documentRuleId);
            return compiledDocument;
        }

        private List<JToken> GetChildRules(JToken ruleTree)
        {
            return ruleTree.SelectToken(DocumentRuleConstant.InnerRules).ToList();
        }

        private void UpdateTargetSection(int formInstanceId, string targetPath, JToken ruleOutput, RuleEventType eventType)
        {
            //if (!ruleOutput.IsNullOrEmpty())  //TODO: Need to set Default JSON if Empty() comes for Update.
            //{
            TargetSectionSyncManager targetSyncManager = new TargetSectionSyncManager(targetPath, formInstanceId, ruleOutput, _sounceDBManager, _requestContext);
            string updatedSectionData = targetSyncManager.GetUpdatedSection(_detail.JSONData, eventType);
            string sectionName = targetPath.GetSectionName();

            if (eventType == RuleEventType.SECTIONLOAD)
            {
                _detail.JSONData = updatedSectionData;
            }
            else
            {
                _sounceDBManager.UpdateProcessedRuleSection(formInstanceId, sectionName, updatedSectionData);
            }
            // }
        }

        private int GetTargetFormInstanceId(string targetPath)
        {
            int formInstanceId = 0;
            string ruleAlise = targetPath.Substring(0, targetPath.IndexOf('['));

            if (_requestContext.RuleAliasesLoadedForSection.ContainsKey(ruleAlise) == false)
            {
                RuleAliasResolver resolver = new RuleAliasResolver(_detail.FormName, _formInstanceId, _folderVersionId, _formInstanceService, _formDesignService, _userId);
                formInstanceId = resolver.Resolve(ruleAlise);
                _requestContext.RuleAliasesLoadedForSection.Add(ruleAlise, formInstanceId);
            }
            else
            {
                formInstanceId = _requestContext.RuleAliasesLoadedForSection[ruleAlise];
            }

            return formInstanceId;
        }
    }
}