using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Extension
{
    class ToBooleanFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            // 1. Get the name of the variable.
            string varName = Utils.GetToken(data, ref from, Constants.END_ARG_ARRAY);
            if (from >= data.Length)
            {
                throw new ArgumentException("Couldn't get variable");
            }

            // 2. Get the current value of the variable.
            ParserFunction func = ParserFunction.GetFunction(varName);
            Parser.Result currentValue = func.GetValue(data, ref from);

            // 3. Take either the length of the underlying tuple or
            // string part if it is defined,
            // or the numerical part converted to a string otherwise.
            string newVal = FunctionHelper.GetArgument(currentValue);

            if (String.Equals(newVal, "true", StringComparison.OrdinalIgnoreCase) || String.Equals(newVal, "yes", StringComparison.OrdinalIgnoreCase))
            {
                newVal = "true";
            }

            Parser.Result newValue = new Parser.Result();
            newValue.String = newVal;

            return newValue;
        }
    }
}
