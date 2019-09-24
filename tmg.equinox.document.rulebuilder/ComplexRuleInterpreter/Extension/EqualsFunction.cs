using Newtonsoft.Json.Linq;
using tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Extension
{
    class EqualsFunction : ParserFunction
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

            // 3. Get the value of second parameter
            Parser.Result newValue = Utils.GetItem(data, ref from);

            string arg1 = FunctionHelper.GetArgument(currentValue);
            string arg2 = FunctionHelper.GetArgument(newValue);
            result.String = string.Equals(arg1, arg2, StringComparison.OrdinalIgnoreCase) ? "true" : "false";
            
            //ParserFunction.AddFunction(varName, new GetVarFunction(newValue));

            return result;
        }
    }
}
