using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.globalUtility;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.ruleinterpreter.evaluator;

namespace tmg.equinox.ruleinterpreter.ruleprocessor
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
