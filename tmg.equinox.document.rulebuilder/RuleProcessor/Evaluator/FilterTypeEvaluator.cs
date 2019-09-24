using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.globalUtility;
using tmg.equinox.document.rulebuilder.model;
using tmg.equinox.document.rulebuilder.evaluator;
using tmg.equinox.document.rulebuilder.ruleprocessor;
using tmg.equinox.document.rulebuilder.executor;
using tmg.equinox.document.rulebuilder.jsonhelper;
using tmg.equinox.document.rulebuilder.ruleProcessor.operandhelper;

namespace tmg.equinox.document.rulebuilder.evaluator
{
    public class FilterTypeEvaluator
    {
        RuleExpressionInput _ruleExpressionInput;

        public FilterTypeEvaluator(RuleExpressionInput ruleExpressionInput)
        {
            _ruleExpressionInput = ruleExpressionInput;
        }
        public JToken GetFilterOutput()
        {
            JToken ruleFilterResult = null;

            switch (_ruleExpressionInput.FilterType)
            {
                case FilterType.distinct:
                    ruleFilterResult = DistinctFilterOutput();
                    break;

                case FilterType.expr:
                    ruleFilterResult = ExpressionFilterOutput();
                    break;

                case FilterType.self:
                    ruleFilterResult = GetSelfFilterOutput();
                    break;

                case FilterType.none:
                    ruleFilterResult = DefaultFilterOutput();
                    break;

                default:
                    break;
            }
            return ruleFilterResult;
        }


        private JToken DistinctFilterOutput()
        {
            JToken jtoken = null;
            List<JToken> _source = _ruleExpressionInput.SourceItemDictionary.First().Value.ToList();
            CollectionExecutionComparer distinctResultEvaluator = new CollectionExecutionComparer(_source, _ruleExpressionInput.KeyColumns);
            List<JToken> distinctOutput = distinctResultEvaluator.Distinct();
            distinctOutput = GetColumnFormattedResult(distinctOutput, _ruleExpressionInput.outputFormat);
            jtoken = distinctOutput.ConvertJtokenListToJToken();
            return jtoken;
        }


        private JToken ExpressionFilterOutput()
        {
            JToken jtoken = null;
            RuleExpressionExecution ruleExecutionWrpper = new RuleExpressionExecution(_ruleExpressionInput);
            List<JToken> expOutput = ruleExecutionWrpper.ProcessRuleExpressions();
            expOutput = GetColumnFormattedResult(expOutput, _ruleExpressionInput.outputFormat);
            jtoken = expOutput.ConvertJtokenListToJToken();
            return jtoken;
        }

        private JToken DefaultFilterOutput()
        {
            JToken jtoken = null;
            List<JToken> defaultResult = _ruleExpressionInput.SourceItemDictionary.First().Value.ToList();
            defaultResult = GetColumnFormattedResult(defaultResult, _ruleExpressionInput.outputFormat);
            jtoken = defaultResult.ConvertJtokenListToJToken();
            return jtoken;
        }


        private JToken GetSelfFilterOutput()
        {
            JToken jtoken = null;
            List<JToken> selfFilterResult = _ruleExpressionInput.SourceMergeExpression.First().Value.LeftOperand.GetSelfOperandValue(_ruleExpressionInput.SourceItemDictionary);
            selfFilterResult = GetColumnFormattedResult(selfFilterResult, _ruleExpressionInput.outputFormat);
            jtoken = selfFilterResult.ConvertJtokenListToJToken();
            return jtoken;
        }

        private List<JToken> GetColumnFormattedResult(List<JToken> ruleExpressionResult, OutputProperties outputFormat)
        {

  if (_ruleExpressionInput.sourceActionOutput == null && outputFormat != null && outputFormat.Columns!=null && outputFormat.Columns.Count > 0) 
            {
                RuleOutputColumnFormatter columnFormatter = new RuleOutputColumnFormatter(ruleExpressionResult, outputFormat);
                ruleExpressionResult = columnFormatter.GetParentChildFormattedOutput();
            }
            return ruleExpressionResult;
        }
    }
}
