using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.model;

namespace tmg.equinox.document.rulebuilder.operatorutility
{
    class RuleOperatorTypeEvaluator
    {
        List<JToken> _source;
        List<JToken> _target; 
        RuleOperatorType _operatorType;
        Dictionary<string, string> _keyColumns;

        public RuleOperatorTypeEvaluator(List<JToken> source, List<JToken> target, RuleOperatorType operatorType,Dictionary<string,string> keyCoulumns)
        {
            _source=source;
            _target=target;
            _operatorType = operatorType;
            _keyColumns = keyCoulumns;
        }

        public List<JToken> GetRuleExpresionsResult()
        {
            List<JToken> expressionResult = new List<JToken>();
            EqualityComparerEvaluator evaluator = new EqualityComparerEvaluator(_source, _target, _keyColumns); // TODO : Need to Confirmed

            switch (_operatorType)
            {
                case RuleOperatorType.contains:
                    break;
                case RuleOperatorType.intersect:
                    expressionResult = evaluator.ResultForIntersection();
                    break;
                case RuleOperatorType.union:
                    expressionResult = evaluator.ResultForUnion();
                    break;
                case RuleOperatorType.coljoin:
                    expressionResult = evaluator.ResultForColJoin();
                    break;
                case RuleOperatorType.except:
                    expressionResult = evaluator.ResultForExcept();
                    break;
                case RuleOperatorType.distinct:
                    //TODO:Impement method for Distinct
                    break;
                default:
                    break;
            }
            return expressionResult;
        }

    }
}
