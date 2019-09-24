using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.ruleinterpreter.ruleprocessor;
using tmg.equinox.ruleinterpreter.pathhelper;
using tmg.equinox.ruleinterpreter.jsonhelper;
using tmg.equinox.forminstanceprocessor.expressionbuilder;

namespace tmg.equinox.expressionbuilder
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

            if (!IsAllSourcesAreEmpty(sources))
                targetJtoken = ruleProcessor.ProcessRule(_compiledRule, sources, target);
            else
                targetJtoken = target;

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

        private Dictionary<string, JToken> GetSourceData()
        {
            RuleSourceDataAdapter sourceDataAdapter = new RuleSourceDataAdapter();
            List<string> ruleAlises = _compiledRule.SourceContainer.RuleSources.Select(sel =>
                                        sel.SourcePath.Substring(0, sel.SourcePath.IndexOf('[')
                                     )).Distinct().ToList();

            Dictionary<string, int> ruleAiseFormInstances = GetRuleAliseBasedFormInstance(ruleAlises);
            Dictionary<string, JToken> ruleSources = sourceDataAdapter.GetRuleSourceData(_folderVersionId, ruleAiseFormInstances, _compiledRule.SourceContainer, _sourceDBManager, _detail,_requestContext);
            return ruleSources;
        }
        
        private JToken GetTargetData()
        {
            string ruleAlise = _compiledRule.Target.TargetPath.GetRuleAlias();
            RuleTargetDataAdapter targetDataAdapter = new RuleTargetDataAdapter();
            List<string> targetRuleAlise = new List<string>();

            targetRuleAlise.Add(ruleAlise);
            Dictionary<string, int> ruleAliseBasedFormInstances = GetRuleAliseBasedFormInstance(targetRuleAlise);
            JToken ruleTarget = targetDataAdapter.GetTargetJSONToken(_folderVersionId, ruleAliseBasedFormInstances, _compiledRule.Target.TargetPath, _sourceDBManager, _detail, _requestContext);

            return ruleTarget;
        }


        private Dictionary<string, int> GetRuleAliseBasedFormInstance(List<string> ruleAlises)
        {
            int resolvedFormInstanceId = 0;
            Dictionary<string, int> ruleAliseFormInstances = new Dictionary<string, int>();
            RuleAliasResolver resolver = new RuleAliasResolver(_detail.FormName, _formInstanceId, _folderVersionId, _formInstanceService, _formDesignService, _userId);

            foreach (string ruleAlise in ruleAlises)
            {
                if (_requestContext.RuleAliasesLoadedForSection.ContainsKey(ruleAlise) == false)
                {
                resolvedFormInstanceId = resolver.Resolve(ruleAlise);
                ruleAliseFormInstances.Add(ruleAlise, resolvedFormInstanceId);
                    _requestContext.RuleAliasesLoadedForSection.Add(ruleAlise, resolvedFormInstanceId);
                }
                else
                {
                    ruleAliseFormInstances.Add(ruleAlise, _requestContext.RuleAliasesLoadedForSection[ruleAlise]);
                }
            }

            return ruleAliseFormInstances;
        }

        private bool IsAllSourcesAreEmpty(Dictionary<string, JToken> sources)
        {
            int emptySourceCount = sources.Values.Where(whr => whr.IsNullOrEmpty()).Count();
            return emptySourceCount == sources.Count;
        }
    }
}