using Newtonsoft.Json.Linq;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Extension
{
    class DateFormatFunction : ParserFunction
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
            Parser.Result whatToAdd = Utils.GetItem(data, ref from);

            // 4. Get the value to be add like Year/Month/Days
            Parser.Result numberToAdd = Utils.GetItem(data, ref from);

            // 5. Get the operation Add/Sub/Multiply
            Parser.Result operation = Utils.GetItem(data, ref from);

            string arg1 = FunctionHelper.GetArgument(currentValue);

            DateTime dt;
            if (DateTime.TryParse(arg1, out dt))
            {
                if (string.Equals("ADD", operation.String, StringComparison.OrdinalIgnoreCase))
                {
                    switch (whatToAdd.String)
                    {
                        case "Year":
                        case "year":
                            dt = dt.AddYears(Convert.ToInt32(numberToAdd.String));
                            break;
                        case "Month":
                        case "month":
                            dt = dt.AddMonths(Convert.ToInt32(numberToAdd.String));
                            break;
                        case "Day":
                        case "day":
                            dt = dt.AddDays(Convert.ToInt32(numberToAdd.String));
                            break;
                    }
                }

                if (currentValue.String != null)
                {
                    result.String = dt.ToShortDateString();
                }
                else
                {
                    result.Token = dt.ToShortDateString();
                }
            }

            return result;
        }
    }
}
