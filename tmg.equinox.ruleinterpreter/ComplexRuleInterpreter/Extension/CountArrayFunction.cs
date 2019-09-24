using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Core;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Extension
{

    class CountArrayFunction : ParserFunction
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
            Parser.Result seperatorTypeResult = Utils.GetItem(data, ref from);
            string seperatorTypeString = FunctionHelper.GetArgument(seperatorTypeResult);

            // 4.Calculate count of array.
            int size = 0;
            size = currentValue.String != null ? currentValue.String.IndexOf(seperatorTypeString) != -1 ?
                currentValue.String.Split(seperatorTypeString.ToCharArray()[0]).Count() : 1
                : size;

            if (size == 0 && currentValue.Token != null)
            {
                string childElement = Convert.ToString(currentValue.Token);
                if (!String.IsNullOrEmpty(childElement))
                    size = childElement.IndexOf(seperatorTypeString) != -1 ? childElement.Split(seperatorTypeString.ToCharArray()[0]).Count() : 1;
                else
                    size = 0;
            }

            Parser.Result newValue = new Parser.Result(size);
            return newValue;
        }
    }
}
