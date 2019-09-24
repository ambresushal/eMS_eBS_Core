using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.evaluator;
using tmg.equinox.document.rulebuilder.executor;
using tmg.equinox.document.rulebuilder.globalUtility;
using tmg.equinox.document.rulebuilder.model;

namespace tmg.equinox.document.rulebuilder.ruleprocessor
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
