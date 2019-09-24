using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Extension
{
    class ContainsFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result result = new Parser.Result();

            // 1. Get the variable name
            string varName = Utils.GetToken(data, ref from, Constants.NEXT_ARG_ARRAY);
            if (from >= data.Length)
            {
                throw new ArgumentException("Couldn't set variable before end of line");
            }

            // 2. Get the current value of the variable.
            ParserFunction func = ParserFunction.GetFunction(varName);
            Parser.Result currentValue = func.GetValue(data, ref from);
            string arg1 = String.Empty;string arg2 = String.Empty;string[] arg3=new String[] { };
            // 3. Get the second parameter value
            Parser.Result varValue = Utils.GetItem(data, ref from);

            // Check if newValue is string and has multiselect format
            if (currentValue.Token != null && currentValue.String == null)
            {
                if (currentValue.Token.ToString().StartsWith("[") && currentValue.Token.ToString().EndsWith("]"))
                {
                    string[] source = currentValue.Token.ToObject<string[]>();
                    if (source != null && source.Count() > 0)
                    {
                        int length = source.Count(); string sourceVal = String.Empty;
                        for (int i = 0; i < length; i++)
                        {
                            sourceVal += source[i] + (i != length - 1 ? "," : "");
                        }
                        arg1= sourceVal;
                        arg2 = FunctionHelper.GetArgument(varValue);
                        arg3 = arg1.Split(',');
                    }
                }
                else
                {
                    // 4. Take either the string part if it is defined,
                    arg1 = FunctionHelper.GetArgument(currentValue);
                    arg2 = FunctionHelper.GetArgument(varValue);
                    arg3 = arg1.Split(',');
                }
            }
            else
            {
                // 4. Take either the string part if it is defined,
                arg1 = FunctionHelper.GetArgument(currentValue);
                arg2 = FunctionHelper.GetArgument(varValue);
                arg3 = arg1.Split(',');
            }
            result.String = Array.Exists(arg3, arg => arg == arg2) ? "true" : "false";
            return result;
        }
    }
}
