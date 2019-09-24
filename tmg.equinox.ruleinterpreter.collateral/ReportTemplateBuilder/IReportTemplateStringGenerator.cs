using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public interface IReportTemplateStringGenerator
    {
        string Generate(string richText);
    }
}
