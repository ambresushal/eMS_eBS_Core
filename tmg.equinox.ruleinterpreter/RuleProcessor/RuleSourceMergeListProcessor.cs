using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.globalUtility;
using tmg.equinox.ruleinterpreter.model;

namespace tmg.equinox.ruleinterpreter.ruleprocessor
{
    public class RuleSourceMergeListProcessor
    {
        CompiledDocumentRule _compiledRule;
        Dictionary<string, JToken> _sources;

        public RuleSourceMergeListProcessor(CompiledDocumentRule rule, Dictionary<string, JToken> sources)
        {
            _compiledRule = rule;
            _sources = sources;
        }

        public JToken GetRuleOutput()
        {
            JToken outputToken = null;

            foreach (SourceMergeAction sourceAction in _compiledRule.SourceContainer.SourceMergeList.SourceMergeActions)
            {
                RuleExpressionInput ruleExpressionInput = RuleEngineGlobalUtility.GetSourceProcessInput(_compiledRule.Target.TargetPath, sourceAction.MergeExpression, new Dictionary<string, string>(), _sources, sourceAction.SourceMergeType, _compiledRule.SourceContainer.SourceMergeList.OutputColumns);
                ruleExpressionInput.sourceActionOutput = outputToken;
                outputToken = ExecuteSourceMergeAction(ruleExpressionInput);
            }
            return outputToken;
        }

        private JToken ExecuteSourceMergeAction(RuleExpressionInput ruleExpressionInput)
        {
            RuleSourceMergeActionProcessor mergeActionProcessor = new RuleSourceMergeActionProcessor(ruleExpressionInput);
            return mergeActionProcessor.ProcessSourceMerging();
        }
    }
}
