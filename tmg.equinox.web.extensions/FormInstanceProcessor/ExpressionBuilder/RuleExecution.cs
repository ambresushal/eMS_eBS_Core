using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.ruleinterpreter.ruleprocessor;
using tmg.equinox.web.FormInstanceProcessor.SourceTargetDataManager.SourceHandler;
using tmg.equinox.web.sourcehandler;
using tmg.equinox.ruleinterpreter.pathhelper;
using tmg.equinox.ruleinterpreter.jsonhelper;
using tmg.equinox.web.FormInstanceProcessor.ExpressionBuilder;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.applicationservices.viewmodels.FormDesign;

namespace tmg.equinox.web.FormInstanceProcessor
{
    public class RuleExecution
    {
        CompiledDocumentRule _compiledRule;
        int _formInstanceId;
        SourceHandlerDBManager _sourceDBManager;
        FormDesignVersionDetail _detail;
        int _folderVersionId;
        IFormDesignService _formDesignService;
        IFormInstanceService _formInstanceService;
        private int? _userId;
        CurrentRequestContext _requestContext;
        public RuleExecution(int? userId, CompiledDocumentRule compiledRule, int formInstanceId, int folderVersionId, SourceHandlerDBManager sourceDBManager, FormDesignVersionDetail detail, IFormDesignService formDesignService, IFormInstanceService formInstanceService, CurrentRequestContext requestContext)
        {
            _compiledRule = compiledRule;
            _sourceDBManager = sourceDBManager;
            _formInstanceId = formInstanceId;
            _detail = detail;
            _folderVersionId = folderVersionId;
            _formInstanceService = formInstanceService;
            _formDesignService = formDesignService;
            _userId = userId;
            _requestContext = requestContext;
        }

        public JToken ProcessRule()
        {
            JToken targetJtoken = null;
            DocumentRulesProcessor ruleProcessor = new DocumentRulesProcessor();
            Dictionary<string, JToken> sources = GetSourceData();
            JToken target = GetTargetData();

            //if (!IsAllSourcesAreEmpty(sources))
            targetJtoken = ruleProcessor.ProcessRule(_compiledRule, sources, target);
            //else
            //    targetJtoken = target;

            // Maintain list of Changed field for activity log
            if (targetJtoken != null)
            {
                if (JToken.DeepEquals(target, targetJtoken) == false)
                {
                    if (!_requestContext.ExpressionRuleActivityLog.ContainsKey(target.Path))
                    {
                        _requestContext.ExpressionRuleActivityLog.Add(target.Path, new { OldValue = target, NewValue = targetJtoken });
                    }

                }
            }
            return targetJtoken;
        }

        public JToken ProcessRule(int targetFormInstanceId)
        {
            JToken targetJtoken = null;
            DocumentRulesProcessor ruleProcessor = new DocumentRulesProcessor();
            Dictionary<string, JToken> sources = GetSourceData();
            JToken target = GetTargetData(targetFormInstanceId);

            //if (!IsAllSourcesAreEmpty(sources))
            targetJtoken = ruleProcessor.ProcessRule(_compiledRule, sources, target);
            //else
            //    targetJtoken = target;

            // Maintain list of Changed field for activity log
            if (targetJtoken != null)
            {
                if (JToken.DeepEquals(target, targetJtoken) == false)
                {
                    if (!_requestContext.ExpressionRuleActivityLog.ContainsKey(target.Path))
                    {
                        _requestContext.ExpressionRuleActivityLog.Add(target.Path, new { OldValue = target, NewValue = targetJtoken });
                    }
                }
            }
            return targetJtoken;
        }

        public JToken ProcessRule(string targetElementPath, string targetValue)
        {
            JToken targetJtoken = null;
            DocumentRulesProcessor ruleProcessor = new DocumentRulesProcessor();
            Dictionary<string, JToken> sources = GetSourceData();
            JToken target = GetTargetData();
            var targetSrc = _compiledRule.SourceContainer.RuleSources.Where(x => x.SourcePath == _compiledRule.Target.TargetPath).FirstOrDefault();
            if (targetSrc != null)
            {
                sources[targetSrc.SourceName] = targetValue;
            }
            if (!IsAllSourcesAreEmpty(sources))
                targetJtoken = ruleProcessor.ProcessRule(_compiledRule, sources, target);
            else
                targetJtoken = target;

            return targetJtoken;
        }


        public JToken ProcessRule(string[] dynamicSource)
        {
            Dictionary<string, JToken> sources = new Dictionary<string, JToken>();
            sources = GetSourceData();

            for (int i = 0; i < dynamicSource.Length; i++)
            {
                string dicKey = "param" + (i + 1);
                if (sources.ContainsKey(dicKey) && !string.IsNullOrEmpty(dynamicSource[i]))
                {
                    JToken dynamicSourceToken = JToken.FromObject(dynamicSource[i]);
                    sources[dicKey] = dynamicSourceToken;
                }
            }

            JToken targetJtoken = null;
            DocumentRulesProcessor ruleProcessor = new DocumentRulesProcessor();
            JToken target = GetTargetData();
            targetJtoken = ruleProcessor.ProcessRule(_compiledRule, sources, target);
            return targetJtoken;
        }



        private Dictionary<string, JToken> GetSourceData()
        {
            RuleSourceDataAdapter sourceDataAdapter = new RuleSourceDataAdapter();
            //EXP_RULES_ENH - add sources
            if (_compiledRule.DocumentRuleTypeID == 5)
            {
                //add sources
                //get document design types in the Folder Group which are not collateral
                int tenantId = 1;
                IEnumerable<FormDesignGroupRowMapModel> designs = _formDesignService.GetFormDesignsForGroup(tenantId, _detail.FormDesignId);
                var sourcesToAdd = designs.Where(a => (a.DocumentLocationID == 1 || a.DocumentLocationID == 2) && a.DocumentDesignTypeID != 11);
                int sourceCount = _compiledRule.SourceContainer.RuleSources.Count;
                if (sourcesToAdd != null && sourcesToAdd.Count() > 0)
                {
                    foreach (var sourceDesign in sourcesToAdd)
                    {
                        var added = _compiledRule.SourceContainer.RuleSources.Where(i => (i.SourceName == sourceDesign.FormDesignName) && (i.SourcePath == (sourceDesign.FormDesignName + "[ROOT]")));
                        if (added == null || added.Count() == 0)
                        {
                            //add complete document as source
                            RuleSourceItem item = new RuleSourceItem();
                            item.SequenceNumber = sourceCount;
                            sourceCount++;
                            item.SourceName = sourceDesign.FormDesignName;
                            item.SourcePath = sourceDesign.FormDesignName + "[ROOT]";
                            _compiledRule.SourceContainer.RuleSources.Add(item);
                            //add master list for aliases as source
                            item = new RuleSourceItem();
                            item.SequenceNumber = sourceCount;
                            sourceCount++;
                            item.SourceName = "DesignAliases_" + sourceDesign.FormDesignName;
                            item.SourcePath = "DesignAliases[" + sourceDesign.FormDesignName + "." + sourceDesign.FormDesignName + "AliasList]";
                            _compiledRule.SourceContainer.RuleSources.Add(item);
                        }
                    }
                }
            }
            List<string> ruleAliases = _compiledRule.SourceContainer.RuleSources.Select(sel =>
                                        sel.SourcePath.Substring(0, sel.SourcePath.IndexOf('[')
                                     )).Distinct().ToList();

            Dictionary<string, int> ruleAiseFormInstances = GetRuleAliasBasedFormInstance(ruleAliases);
            Dictionary<string, JToken> ruleSources = sourceDataAdapter.GetRuleSourceData(_folderVersionId, ruleAiseFormInstances, _compiledRule.SourceContainer, _sourceDBManager, _detail, _requestContext);
            return ruleSources;
        }

        private JToken GetTargetData()
        {
            string ruleAlias = _compiledRule.Target.TargetPath.GetRuleAlias();
            RuleTargetDataAdapter targetDataAdapter = new RuleTargetDataAdapter();
            List<string> targetRuleAlias = new List<string>();

            targetRuleAlias.Add(ruleAlias);
            Dictionary<string, int> ruleAliasBasedFormInstances = GetRuleAliasBasedFormInstance(targetRuleAlias);
            JToken ruleTarget = targetDataAdapter.GetTargetJSONToken(_folderVersionId, ruleAliasBasedFormInstances, _compiledRule.Target.TargetPath, _sourceDBManager, _detail, _requestContext);

            return ruleTarget;
        }

        private JToken GetTargetData(int targetFormInstanceId)
        {
            string ruleAlias = _compiledRule.Target.TargetPath.GetRuleAlias();
            RuleTargetDataAdapter targetDataAdapter = new RuleTargetDataAdapter();
            List<string> targetRuleAlias = new List<string>();

            targetRuleAlias.Add(ruleAlias);
            Dictionary<string, int> ruleAliasBasedFormInstances = new Dictionary<string, int>();
            ruleAliasBasedFormInstances.Add(ruleAlias, targetFormInstanceId);
            JToken ruleTarget = targetDataAdapter.GetTargetJSONToken(_folderVersionId, ruleAliasBasedFormInstances, _compiledRule.Target.TargetPath, _sourceDBManager, _detail, _requestContext);
            return ruleTarget;
        }


        private Dictionary<string, int> GetRuleAliasBasedFormInstance(List<string> ruleAliases)
        {
            int resolvedFormInstanceId = 0;
            Dictionary<string, int> ruleAliasFormInstances = new Dictionary<string, int>();
            RuleAliasResolver resolver = new RuleAliasResolver(_detail.FormName, _formInstanceId, _detail.FormDesignId, _folderVersionId, _formInstanceService, _formDesignService, _userId);

            foreach (string ruleAlias in ruleAliases)
            {
                if (_requestContext.RuleAliasesLoadedForSection.ContainsKey(ruleAlias) == false)
                {
                    resolvedFormInstanceId = resolver.Resolve(ruleAlias);
                    ruleAliasFormInstances.Add(ruleAlias, resolvedFormInstanceId);
                    _requestContext.RuleAliasesLoadedForSection.Add(ruleAlias, resolvedFormInstanceId);
                }
                else
                {
                    ruleAliasFormInstances.Add(ruleAlias, _requestContext.RuleAliasesLoadedForSection[ruleAlias]);
                }
            }

            return ruleAliasFormInstances;
        }

        public Dictionary<string, List<int>> GetRuleAliasBasedFormInstanceForMultipleTargets(List<string> ruleAliases)
        {
            Dictionary<string, List<int>> ruleAliasFormInstances = new Dictionary<string, List<int>>();
            RuleAliasResolver resolver = new RuleAliasResolver(_detail.FormName, _formInstanceId, _detail.FormDesignId, _folderVersionId, _formInstanceService, _formDesignService, _userId);

            foreach (string ruleAlias in ruleAliases)
            {
                resolver.ResolveMultiple(ruleAlias);

                if (_requestContext.MultipleRuleAliasesLoadedForSection.ContainsKey(ruleAlias) == false)
                {
                    List<int> resolvedFormInstanceIds = resolver.ResolveMultiple(ruleAlias);
                    ruleAliasFormInstances.Add(ruleAlias, resolvedFormInstanceIds);
                    _requestContext.MultipleRuleAliasesLoadedForSection.Add(ruleAlias, resolvedFormInstanceIds);
                }
                else
                {
                    ruleAliasFormInstances.Add(ruleAlias, _requestContext.MultipleRuleAliasesLoadedForSection[ruleAlias]);
                }
            }
            return ruleAliasFormInstances;
        }

        private bool IsAllSourcesAreEmpty(Dictionary<string, JToken> sources)
        {
            int emptySourceCount = sources.Values.Where(whr => whr.IsNullOrEmpty()).Count();
            return emptySourceCount == sources.Count;
        }
    }
}