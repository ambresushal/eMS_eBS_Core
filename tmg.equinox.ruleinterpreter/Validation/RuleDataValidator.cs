using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpreter.validation
{
    public class RuleDataValidator
    {

        public Dictionary<string, IEnumerable<string>> InValidElementPaths(Dictionary<string, List<string>> documentElementPath, Dictionary<string, List<string>> ruleElementPath)
        {
            return (from ruleElements in ruleElementPath
                    join documentElements in documentElementPath
                    on ruleElements.Key equals documentElements.Key
                    select new KeyValuePair<string, IEnumerable<string>>(ruleElements.Key, ruleElements.Value.Except(documentElements.Value))
                   ).ToDictionary(x => x.Key, x => x.Value);
        }

        // Add method to validate Source Keys.


    }
}
