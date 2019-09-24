using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.ruleinterpreter.executor;

namespace tmg.equinox.ruleinterpreter.evaluator
{
    class CompareValueOperatorEvaluator
    {

        List<JToken> _source;
        string _leftOperand;
        string _rightOperand;
        RuleOperatorType _operatorType;

        public CompareValueOperatorEvaluator(List<JToken> source, RuleOperatorType operatorType, string leftOperand, string rightOperand)
        {
            _source = source;
            _leftOperand = leftOperand;
            _rightOperand = rightOperand;
            _operatorType = operatorType;
        }

        public List<JToken> GetCompareValueExpressionResult()
        {
            List<JToken> expressionResult = new List<JToken>();
            CollectionValueCompareExecution arithmaticComparer = new CollectionValueCompareExecution(_source, _leftOperand, _rightOperand);
            switch (_operatorType)
            {
                case RuleOperatorType.equalto:
                    expressionResult=arithmaticComparer.EqualTo();
                    break;

                case RuleOperatorType.lessthan:
                    expressionResult=arithmaticComparer.LessThan();
                    break;
                case RuleOperatorType.lessthanequalto:
                    expressionResult=arithmaticComparer.LessThanEqualTo();
                    break;
                case RuleOperatorType.greaterthan:
                    expressionResult=arithmaticComparer.GreaterThan();
                    break;
                case RuleOperatorType.greaterthanequalto:
                    expressionResult=arithmaticComparer.GreaterThanEqualTo();
                    break;
                case RuleOperatorType.contains:
                    expressionResult=arithmaticComparer.Contains();
                    break;
                case RuleOperatorType.notequalto:
                    expressionResult = arithmaticComparer.NotEqualTo();
                    break;
                default:
                    break;
            }
            return expressionResult;
        }
    }
}
