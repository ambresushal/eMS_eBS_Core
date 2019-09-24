using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.reporting.Base.Interface;
using tmg.equinox.reporting.Base.Model;

namespace tmg.equinox.reporting.Base
{
    public class BaseRule : IRule
    {
        RuleInfo _rules;
        public RuleInfo Execute(RuleInfo rules)
        {
            _rules = rules;
            switch (_rules.RuleName)
            {
                case "Rule1":
                    Rule1();
                    break;
                default:
                    break;
            }
            return _rules;
        }

        protected void Rule1()
        {

        }
    }
}
