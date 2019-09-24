using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Extension
{
    class ConvertTypeToFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            // 1. Get the name of the variable.
            string varName = Utils.GetToken(data, ref from, Constants.NEXT_ARG_ARRAY);
            if (from >= data.Length)
            {
                throw new ArgumentException("Couldn't get variable");
            }

            // 2. Get the current value of the variable.
            ParserFunction func = ParserFunction.GetFunction(varName);
            Parser.Result currentValue = func.GetValue(data, ref from);

            // 3. Get the value of second parameter
            Parser.Result dataTypeResult = Utils.GetItem(data, ref from);
            string dataTypeString= FunctionHelper.GetArgument(dataTypeResult);

            Parser.Result typeCastResult = FunctionHelper.ConvertToType(currentValue, dataTypeString);

            return typeCastResult;
        }
    }
}
