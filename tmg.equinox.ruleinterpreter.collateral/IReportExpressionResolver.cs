using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public interface IReportExpressionResolver
    {
        DocumentAliases GetAliases(List<string> expressions);

        ExpressionTemplate GetExpressionTemplate(string expression);
    }
}
