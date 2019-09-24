using Newtonsoft.Json.Linq;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Extension
{
    class ReplaceFunction : ParserFunction
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

            // 4. Get the value to be replaced
            Parser.Result repValue = Utils.GetItem(data, ref from);

            // 4. Take either the string part if it is defined,
            // or the numerical part converted to a string otherwise.

            string arg1 = FunctionHelper.GetArgument(currentValue);
            string arg2 = FunctionHelper.GetArgument(newValue);
            string arg3 = FunctionHelper.GetArgument(repValue);
            
            // 5. The variable becomes a string after adding a string to it.
            if (currentValue.String != null)
            {
                result.String = arg1.Replace(arg2, arg3);
            }
            else
            {
                result.Token = arg1.Replace(arg2, arg3);
            }

            //ParserFunction.AddFunction(varName, new GetVarFunction(newValue));

            return result;
        }
    }
}
