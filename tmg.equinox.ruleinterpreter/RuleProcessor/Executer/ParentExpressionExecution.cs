using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.evaluator;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.ruleinterpreter.executor;
using tmg.equinox.ruleinterpreter.ruleProcessor.operandhelper;

namespace tmg.equinox.ruleinterpreter.executor
{
    public class ParentExpressionExecution
    {

        RuleFilterExpression _ruleExpression;
        Dictionary<string, JToken> _sources;
        Dictionary<string, string> _keyColumns;
        Dictionary<string, JToken> _derivedExpressionsOutput;
        JToken _parentExpressionResult;
        OperandValueEvaluator operandTypeEvaluator;


        public ParentExpressionExecution(Dictionary<string, JToken> sources, RuleFilterExpression ruleExpression, Dictionary<string, string> keyColumns, Dictionary<string, JToken> derivedExpressionsOutput, JToken parentExpressionResult)
        {
            _sources = sources;
            _ruleExpression = ruleExpression;
            _keyColumns = keyColumns;
            _derivedExpressionsOutput = derivedExpressionsOutput;
            _parentExpressionResult = parentExpressionResult;
            operandTypeEvaluator = new OperandValueEvaluator(sources, derivedExpressionsOutput);
        }

        public List<JToken> CrossJoinExecution()
        {
            List<JToken> source = operandTypeEvaluator.GetOperandValue(_ruleExpression.LeftOperand);
            List<JToken> target = operandTypeEvaluator.GetOperandValue(_ruleExpression.RightOperand);
            string propertyName = _ruleExpression.RightOperand.OperandPath;
            CrossJoinExecution executer = new CrossJoinExecution(source, propertyName, target);
            return executer.CrossJoin();
        }

        public List<JToken> CollectionCompareExecution()
        {
            List<JToken> source = operandTypeEvaluator.GetOperandValue(_ruleExpression.LeftOperand);
            List<JToken> target = operandTypeEvaluator.GetOperandValue(_ruleExpression.RightOperand);
            EqualityComparerOperatorEvaluator operatorEvaluator = new EqualityComparerOperatorEvaluator(source, target, _ruleExpression.Operator, _keyColumns);
            return operatorEvaluator.GetEqualityComparerResult();
        }

        public List<JToken> CollectionValueCompareExecution()
        {
            List<JToken> source = _ruleExpression.LeftOperand.GetSelfOperandValue(_sources);  //GetSelfOperandValue(_ruleExpression.LeftOperand);
            string propertyName = _ruleExpression.LeftOperand.OperandPath;
            string propertyValue = _ruleExpression.RightOperand.GetPropertyOperandValue(_sources);//  //GetPropertyOperandValue(_ruleExpression.RightOperand);
            CompareValueOperatorEvaluator compareValueEvaluator = new CompareValueOperatorEvaluator(source, _ruleExpression.Operator, propertyName, propertyValue);
            return compareValueEvaluator.GetCompareValueExpressionResult();
        }
    }
}
