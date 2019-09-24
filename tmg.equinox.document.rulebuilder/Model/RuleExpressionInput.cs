using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.document.rulebuilder.model
{
    public class RuleExpressionInput
    {
        public string ExpressionKeyName { get; set; }
        public Dictionary<string, JToken> SourceItemDictionary { get; set; }
        public FilterType FilterType { get; set; }
        public Dictionary<string, RuleFilterExpression> SourceMergeExpression { get; set; }
        public Dictionary<string, string> KeyColumns { get; set; }
        public JToken sourceActionOutput { get; set; }
        public OutputProperties outputFormat { get; set; }
    }
}
