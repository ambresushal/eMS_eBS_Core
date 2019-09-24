using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.model;
using tmg.equinox.document.rulebuilder.evaluator;
using tmg.equinox.document.rulebuilder.globalUtility;

namespace tmg.equinox.document.rulebuilder.ruleprocessor
{
   public class RuleFilterProcessor
    {
       RuleFilterItem _ruleFilterItem;
       Dictionary<string, JToken> _sources;

       public RuleFilterProcessor(RuleFilterItem ruleFilterItem, Dictionary<string, JToken> sources)
       {
           _ruleFilterItem = ruleFilterItem;
           _sources = sources;
       }

       public JToken ProcessFilterItem()
       {
           RuleExpressionInput ruleExpressionInput = RuleEngineGlobalUtility.GetSourceProcessInput(_ruleFilterItem.FilterName,_ruleFilterItem.Expression, _ruleFilterItem.DistinctKeyColumn, _sources, _ruleFilterItem.FilterType, null); 
           FilterTypeEvaluator fiterTypeEvaluator = new FilterTypeEvaluator(ruleExpressionInput);
           return fiterTypeEvaluator.GetFilterOutput();
       }
    }
}
