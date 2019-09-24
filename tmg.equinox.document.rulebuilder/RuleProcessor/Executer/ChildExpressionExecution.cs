using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.evaluator;
using tmg.equinox.document.rulebuilder.model;
using tmg.equinox.document.rulebuilder.ruleProcessor.operandhelper;

namespace tmg.equinox.document.rulebuilder.executor
{
  public class ChildExpressionExecution
    {

        RuleFilterExpression _ruleExpression;
        Dictionary<string, JToken> _sources;
        Dictionary<string, string> _keyColumns;
        Dictionary<string, JToken> _derivedExpressionsOutput;
        JToken _parentExpressionResult;
        OperandValueEvaluator operandTypeEvaluator;

        public ChildExpressionExecution(Dictionary<string, JToken> sources, RuleFilterExpression ruleExpression, Dictionary<string, string> keyColumns, Dictionary<string, JToken> derivedExpressionsOutput, JToken parentExpressionResult)
        {
            _sources = sources;
            _ruleExpression = ruleExpression;
            _keyColumns = keyColumns;
            _derivedExpressionsOutput = derivedExpressionsOutput;
            _parentExpressionResult = parentExpressionResult;
            operandTypeEvaluator = new OperandValueEvaluator(sources, derivedExpressionsOutput);
        }

        public List<JToken> ProcessChildExpression()
        {
            List<JToken> parentOutput = _parentExpressionResult.ToList();

            switch (_ruleExpression.ExecutionType)
            {
                case ExecutionType.crossjoin:
                    parentOutput = GetChildCrossJoin();
                    break;
                case ExecutionType.collectioncomparer:
                    parentOutput = GetChildCollectionCompare();  //Tentative Implementation - Will be modified based on scenario for Child Expression
                    break;
                case ExecutionType.collectionvaluecomparer:
                    parentOutput = GetChildCollectionValue(); //Tentative Implementation - Will be modified based on scenario for Child Expression
                    break;
                case ExecutionType.none:
                    break;
                default:
                    break;
            }
            return parentOutput;
        }

        private List<JToken> GetChildCrossJoin()
        {
            List<JToken> parentOutput = _parentExpressionResult.ToList();
            List<JToken> target = operandTypeEvaluator.GetOperandValue(_ruleExpression.RightOperand);
            string propertyName = _ruleExpression.RightOperand.OperandPath;

            foreach (JToken jToken in parentOutput)
            {
                List<JToken> source = jToken.SelectToken(_ruleExpression.LeftOperand.OperandPath).ToList();
                CrossJoinExecution executer = new CrossJoinExecution(source, propertyName, target);
                JArray jarray = new JArray(executer.CrossJoin());
                jToken.SelectToken(_ruleExpression.LeftOperand.OperandPath).Replace(jarray);
            }
            return parentOutput;
        }

        private List<JToken> GetChildCollectionCompare()
        {
            List<JToken> parentOutput = _parentExpressionResult.ToList();
            foreach (JToken jToken in parentOutput)
            {
                List<JToken> source = jToken.SelectToken(_ruleExpression.LeftOperand.OperandPath).ToList();
                List<JToken> target = jToken.SelectToken(_ruleExpression.RightOperand.OperandPath).ToList();
                EqualityComparerOperatorEvaluator operatorEvaluator = new EqualityComparerOperatorEvaluator(source, target, _ruleExpression.Operator, _keyColumns);
                List<JToken> compareResult = operatorEvaluator.GetEqualityComparerResult();
                JArray jarray = new JArray(compareResult);
                jToken.SelectToken(_ruleExpression.LeftOperand.OperandPath).Replace(jarray);
            }
            return parentOutput;
        }

        private List<JToken> GetChildCollectionValue()
        {
            List<JToken> parentOutput = _parentExpressionResult.ToList();
            string propertyName = _ruleExpression.LeftOperand.OperandPath;
            string propertyValue = _ruleExpression.RightOperand.GetPropertyOperandValue(_sources);

            foreach (JToken jToken in parentOutput)
            {
                List<JToken> source = jToken.SelectToken(_ruleExpression.LeftOperand.OperandPath).ToList();
                CompareValueOperatorEvaluator arithMatichEvaluator = new CompareValueOperatorEvaluator(source, _ruleExpression.Operator, propertyName, propertyValue);
                List<JToken> compareValueResult = arithMatichEvaluator.GetCompareValueExpressionResult();
                JArray jarray = new JArray(compareValueResult);
                jToken.SelectToken(_ruleExpression.LeftOperand.OperandPath).Replace(jarray);
            }
            return parentOutput;
        }
    }
}
