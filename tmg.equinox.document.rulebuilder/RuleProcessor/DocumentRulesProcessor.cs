using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.globalUtility;
using tmg.equinox.document.rulebuilder.model;
using tmg.equinox.document.rulebuilder.rulecompiler;
using tmg.equinox.document.rulebuilder.jsonhelper;
using tmg.equinox.document.rulebuilder.ruletargetsync;
using tmg.equinox.document.rulebuilder.RuleProcessor;

namespace tmg.equinox.document.rulebuilder.ruleprocessor
{
    public class DocumentRulesProcessor
    {

        public JToken ProcessRule(CompiledDocumentRule rule, Dictionary<string, JToken> sources, JToken target)
        {
            JToken ruleSourceOutput = null;
            RuleProcessor ruleProcessor = new RuleProcessor(rule, sources);
            Dictionary<string, JToken> processedSources = ruleProcessor.ProcessSources();
        
            if (GetSourceMergeActionType(rule) == FilterType.script)
            {
                SourceMergeScriptProcessor scriptProcessor = new SourceMergeScriptProcessor(target, processedSources, rule);
                ruleSourceOutput = scriptProcessor.Process();
            }
            else
            {
                RuleSourceMergeListProcessor sourceMergeListProcessor = new RuleSourceMergeListProcessor(rule, processedSources);
                ruleSourceOutput = sourceMergeListProcessor.GetRuleOutput();
            }

            return ruleSourceOutput.IsNullOrEmpty() ? null : ruleSourceOutput;
        }

        private FilterType GetSourceMergeActionType(CompiledDocumentRule rule)
        {
            FilterType type = FilterType.none;
            var mergeActions = rule.SourceContainer.SourceMergeList.SourceMergeActions.FirstOrDefault();
            if (mergeActions != null)
            {
                type = mergeActions.SourceMergeType;
            }
            return type;
        }

        private JToken GetTargetSyncOutput(JToken target, JToken ruleOutput, Dictionary<string, string> keyColumns)
        {
            JToken targetJSONToken = null;
            UpdateTargetElement targetElement = new UpdateTargetElement(targetJSONToken, ruleOutput, keyColumns);
            List<JToken> targetToken = targetElement.GetRuleTargetOutput();
            return targetToken.ConvertJtokenListToJToken();
        }

    }
}

