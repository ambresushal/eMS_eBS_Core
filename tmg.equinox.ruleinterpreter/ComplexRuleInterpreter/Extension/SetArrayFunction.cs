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
    class SetArrayFunction : ParserFunction
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
            if (ParserFunction.FunctionExists(varName))
            {
                ParserFunction func = ParserFunction.GetFunction(varName);
                Parser.Result currentValue = func.GetValue(data, ref from);

                // 3. Get the value of second parameter
                Parser.Result newValue = Utils.GetItem(data, ref from);

                // 4. Set Text
                currentValue.Copy(newValue);
            }
            else
            {
                // 3. Get the value of second parameter
                Parser.Result newValue = Utils.GetItem(data, ref from);
                // Check if newValue is string and has multiselect format
                if (newValue.Token == null && newValue.String != null)
                {
                    if (newValue.String.StartsWith("[") && newValue.String.EndsWith("]"))
                    {
                        JArray array = JArray.Parse(newValue.String);
                        SourceManager.Set(Thread.CurrentThread, "target", array);
                    }
                }
                else
                    SourceManager.Set(Thread.CurrentThread, "target", newValue.Token);
            }

            return result;
        }
    }
}
