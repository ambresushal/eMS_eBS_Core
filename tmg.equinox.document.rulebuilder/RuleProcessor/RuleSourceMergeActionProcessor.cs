using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.globalUtility;
using tmg.equinox.document.rulebuilder.model;
using tmg.equinox.document.rulebuilder.evaluator;

namespace tmg.equinox.document.rulebuilder.ruleprocessor
{
    public class RuleSourceMergeActionProcessor
    {
        RuleExpressionInput _mergeActionInput;
        
        public RuleSourceMergeActionProcessor(RuleExpressionInput mergeActionInput)
        {
            _mergeActionInput = mergeActionInput;
        }

        public JToken ProcessSourceMerging()
        {
            FilterTypeEvaluator filterTypeEvaluator = new FilterTypeEvaluator(_mergeActionInput);
            return filterTypeEvaluator.GetFilterOutput();
        }
    }
}
