using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.evaluator;
using tmg.equinox.ruleinterpreter.executor;
using tmg.equinox.ruleinterpreter.globalUtility;
using tmg.equinox.ruleinterpreter.model;

namespace tmg.equinox.ruleinterpreter.ruleprocessor
{
    public class RuleFilterMergeActionProcessor
    {
        RuleExpressionInput _mergeActionInput;

        public RuleFilterMergeActionProcessor(RuleExpressionInput mergeActionInput)
        {
            _mergeActionInput = mergeActionInput;
        }

        public JToken ProcessFilterMerging()
        {
            JToken filterMergeToken;
            FilterTypeEvaluator filterTypeEvaluator = new FilterTypeEvaluator(_mergeActionInput);
            filterMergeToken = filterTypeEvaluator.GetFilterOutput();
            return filterMergeToken;
        }
    }
}
