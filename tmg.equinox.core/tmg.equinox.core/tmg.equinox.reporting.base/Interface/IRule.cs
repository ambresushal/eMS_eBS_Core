using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.reporting.Base.Model;

namespace tmg.equinox.reporting.Base.Interface
{
    public interface IRule
    {
        RuleInfo Execute(RuleInfo rules);
    }
}
