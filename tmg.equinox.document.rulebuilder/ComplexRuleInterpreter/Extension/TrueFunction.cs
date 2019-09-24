using tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Extension
{
    class TrueFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result result = new Parser.Result();
            result.String = "true";
            return result;
        }
    }
}
