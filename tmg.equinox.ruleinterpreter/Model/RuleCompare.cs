using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpreter.model
{
    public class RuleCompare
    {
        public string RuleCompareTo { get; set; }
        public string RuleCompareWith { get; set; }
        public List<string> Sources { get; set; }
        public List<string> Targets { get; set; }
    }
}
