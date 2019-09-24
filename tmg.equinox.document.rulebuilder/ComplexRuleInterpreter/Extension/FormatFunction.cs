using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Core;

namespace tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Extension
{
    public class FormatFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result result = new Parser.Result();

            // 1. Get the name of the variable.
            string varName = Utils.GetToken(data, ref from, Constants.NEXT_ARG_ARRAY);
            if (from >= data.Length)
            {
                throw new ArgumentException("Couldn't find variable");
            }

            // 2. Get the current value of the variable.
            ParserFunction func = ParserFunction.GetFunction(varName);
            Parser.Result currentValue = func.GetValue(data, ref from);

            // 3. Get Index, if specified
            Parser.Result filterValue = Utils.GetItem(data, ref from);
            Parser.Result filters = FunctionHelper.GetFilter(currentValue, filterValue);

            // 4. Get the value of second parameter i.e. Format
            Parser.Result formatValue = Utils.GetItem(data, ref from);

            // 5. Get the value of third parameter i.e. Additional Parameter
            Parser.Result paramValue = Utils.GetItem(data, ref from);

            string format = FunctionHelper.GetArgument(formatValue);
            string param = FunctionHelper.GetArgument(paramValue);

            result.String = FormatHelper.EvaluateFormat(format, filters, param);

            return result;
        }
    }
}
