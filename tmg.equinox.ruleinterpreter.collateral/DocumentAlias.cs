using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class DocumentAlias
    {
        public string DocumentDesignName { get; set; }
        public string Alias { get; set; }
        public string[] Values { get; set; }
        public string Value { get; set; }
        public ValueType ValueType { get; set; }
        public string Operator { get; set; }
        public string ActualValue { get; set; }
        public ValueType ActualValueType { get; set; }
        public string[] ActualValues { get; set; }
        public bool IsValid { get; set; }
    }
}
