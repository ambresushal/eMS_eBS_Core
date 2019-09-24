using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.globalUtility;
using tmg.equinox.document.rulebuilder.model;
using tmg.equinox.document.rulebuilder.operatorutility;
using tmg.equinox.document.rulebuilder.ruleprocessor;

namespace tmg.equinox.document.rulebuilder.OperatorUtility
{
    public class RuleFilterTypeEvaluator
    {

        List<JToken> _source;
        FilterType _filterType;
        Dictionary<string, string> _keyColumns;
        Dictionary<string, RuleFilterExpression> _ruleExpressions;
        Dictionary<string, JToken> _sourceItems;

        public RuleFilterTypeEvaluator(JToken source, FilterType filterType, Dictionary<string, string> keyColumns)
        {
            _source = source.FirstOrDefault().Values().ToList();
            _filterType = filterType;
            _keyColumns = keyColumns;
        }

        //This constructor is not in used will be used later

        public RuleFilterTypeEvaluator(Dictionary<string, JToken> sourceItems, Dictionary<string, RuleFilterExpression> ruleExpressions, Dictionary<string, string> keyColumns)
        {
            _sourceItems = sourceItems;
            _keyColumns = keyColumns;
            _ruleExpressions = ruleExpressions;
        }

        public JToken GetRuleFilterResult()
        {
            EqualityComparerEvaluator selfEvaluator = new EqualityComparerEvaluator(_source, _keyColumns); // TODO : Need to Confirmed
            JToken ruleFilterResult=null;

            switch (_filterType)
            {
                case FilterType.distinct:
                    ruleFilterResult = RuleEngineGlobalUtility.JTokenCollectionToJObject(selfEvaluator.ResultForDistinct(), "");
                    break;
                case FilterType.expr:
                    RuleExpressionExecutionWrapper ruleExecutionWrpper = new RuleExpressionExecutionWrapper(_sourceItems, _ruleExpressions, _keyColumns);
                    ruleExecutionWrpper.ProcessRuleExpressions();
                    break;
                case FilterType.Other:
                    break;
                case FilterType.none:
                    break;
                default:
                    break;
            }
            return ruleFilterResult;
        }
    }
}
