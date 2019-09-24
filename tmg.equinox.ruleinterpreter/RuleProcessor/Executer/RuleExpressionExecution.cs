using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.globalUtility;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.ruleinterpreter.evaluator;
using tmg.equinox.ruleinterpreter.executor;
using tmg.equinox.ruleinterpreter.jsonhelper;
using tmg.equinox.ruleinterpreter.ruleprocessor;

namespace tmg.equinox.ruleinterpreter.executor
{
    public class RuleExpressionExecution
    {
        Dictionary<string, JToken> _sources;
        Dictionary<string, RuleFilterExpression> _ruleExpressions;
        Dictionary<string, JToken> _derivedExpressionsOutput;
        Dictionary<string, string> _keyColumns;
        JToken _parentResult;

        public RuleExpressionExecution(RuleExpressionInput ruleExpressionInput)
        {
            _sources = ruleExpressionInput.SourceItemDictionary;
            _ruleExpressions = ruleExpressionInput.SourceMergeExpression;
            _keyColumns = ruleExpressionInput.KeyColumns;
            _parentResult = ruleExpressionInput.sourceActionOutput;
        }

        public List<JToken> ProcessRuleExpressions()
        {
            string expressionOutputKey = string.Empty;
            _derivedExpressionsOutput = new Dictionary<string, JToken>();
            foreach (var item in _ruleExpressions)
            {
                List<JToken> OutputTokens = new List<JToken>();
                
                if (IsChildExpression(item.Value))
                {
                    ChildExpressionExecution childExpressionProcessor = new ChildExpressionExecution(_sources, item.Value, _keyColumns, _derivedExpressionsOutput, _parentResult);
                    OutputTokens = childExpressionProcessor.ProcessChildExpression();
                }

                else
                {
                    OutputTokens = ProcessRuleExpression(item.Value);
                }

                JToken expressionOutputToken = OutputTokens.ConvertJtokenListToJToken();
                _derivedExpressionsOutput.Add(item.Key, expressionOutputToken);
                expressionOutputKey = item.Key;
            }

            return _derivedExpressionsOutput[expressionOutputKey].ToList();
        }

        private List<JToken> ProcessRuleExpression(RuleFilterExpression ruleExpression)
        {
            List<JToken> ruleOutput = new List<JToken>();
            ParentExpressionExecution executionTypeProcessor = new ParentExpressionExecution(_sources, ruleExpression, _keyColumns, _derivedExpressionsOutput, _parentResult);

            switch (ruleExpression.ExecutionType)
            {
                case ExecutionType.crossjoin:
                    ruleOutput = executionTypeProcessor.CrossJoinExecution();
                    break;

                case ExecutionType.collectioncomparer:
                    ruleOutput = executionTypeProcessor.CollectionCompareExecution();
                    break;

                case ExecutionType.collectionvaluecomparer:
                    ruleOutput = executionTypeProcessor.CollectionValueCompareExecution();
                    break;

                case ExecutionType.none:
                    break;

                default:
                    break;
            }

            return ruleOutput;
        }

        private bool IsChildExpression(RuleFilterExpression ruleFilterExpression) // TODO : Optimized for static Value child
        {
            return (ruleFilterExpression.LeftOperand.OperandValue == "child" || ruleFilterExpression.RightOperand.OperandValue == "child");
        }
    }
}
