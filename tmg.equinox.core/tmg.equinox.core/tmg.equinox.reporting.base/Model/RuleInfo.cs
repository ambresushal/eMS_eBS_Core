using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.reporting.Base.Model
{
    public class RuleInfo
    {
        public string RuleName { get; set; }
        public KeyValuePair<string, object> Data { get; set; }
    }
}
