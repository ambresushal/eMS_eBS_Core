using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.globalUtility;
using tmg.equinox.document.rulebuilder.model;
using tmg.equinox.document.rulebuilder.ruleProcessor.jsonutility;

namespace tmg.equinox.document.rulebuilder.executor
{
    public class RuleOutputColumnFormatter
    {
        List<JToken> _ruleExpressionResult;
        OutputProperties _outputFormat;

        public RuleOutputColumnFormatter(List<JToken> ruleExpressionResult, OutputProperties outputFormat)
        {
            _ruleExpressionResult = ruleExpressionResult;
            _outputFormat = outputFormat;
        }

        public List<JToken> GetParentChildFormattedOutput()
        {
            _ruleExpressionResult = _ruleExpressionResult.GetAdHocColumnDetails(_outputFormat.Columns.Values.FirstOrDefault());//TODO: Optimized 
            _ruleExpressionResult = _outputFormat.Children.Count > 0 ? _ruleExpressionResult.AppendChildren(_outputFormat.Children) : _ruleExpressionResult;
            return _ruleExpressionResult;
        }
    }
}
