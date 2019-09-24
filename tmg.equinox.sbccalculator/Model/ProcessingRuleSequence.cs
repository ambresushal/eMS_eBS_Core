using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.sbccalculator.Model
{
    public class ProcessingRuleSequence
    {
        public string ProcessingRule { get; set; }
        public Dictionary<string, int> SequenceDict;

        public ProcessingRuleSequence()
        {
            SequenceDict = new Dictionary<string, int>();
        }
    }
}
