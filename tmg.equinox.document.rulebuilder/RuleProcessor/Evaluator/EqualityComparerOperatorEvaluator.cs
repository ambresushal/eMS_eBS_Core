using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.executor;
using tmg.equinox.document.rulebuilder.model;

namespace tmg.equinox.document.rulebuilder.evaluator
{
    class EqualityComparerOperatorEvaluator
    {
        List<JToken> _source;
        List<JToken> _target;
        RuleOperatorType _operatorType;
        Dictionary<string, string> _keyColumns;

        public EqualityComparerOperatorEvaluator(List<JToken> source, List<JToken> target, RuleOperatorType operatorType, Dictionary<string, string> keyCoulumns)
        {
            _source = source;
            _target = target;
            _operatorType = operatorType;
            _keyColumns = keyCoulumns;
        }

        public List<JToken> GetEqualityComparerResult()
        {
            List<JToken> expressionResult = new List<JToken>();
            CollectionExecutionComparer evaluator = new CollectionExecutionComparer(_source, _target, _keyColumns); // TODO : Need to Confirmed

            switch (_operatorType)
            {
                case RuleOperatorType.intersect:
                    expressionResult = evaluator.Intersection();
                    break;
                case RuleOperatorType.union:
                    expressionResult = evaluator.Union();
                    break;
                case RuleOperatorType.coljoin:
                    expressionResult = evaluator.ColJoin();
                    break;
                case RuleOperatorType.except:
                    expressionResult = evaluator.Except();
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
