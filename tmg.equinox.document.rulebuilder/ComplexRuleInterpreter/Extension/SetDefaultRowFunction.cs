using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Core;
using tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Helper;
using tmg.equinox.document.rulebuilder.operatorutility;

namespace tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Extension
{
    class SetDefaultRowFunction : ParserFunction
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
                Parser.Result defaultRow = Utils.GetItem(data, ref from);
                //Get the value of second parameter
                JToken targetVal = SourceManager.Get(Thread.CurrentThread, "target");
                //convert to JToken
                if (targetVal == null || !targetVal.HasValues)
                {
                    //parse the defaultRow string to JToken
                    JArray arr = new JArray();
                    arr.Add(JToken.Parse(defaultRow.String));
                    SourceManager.Set(Thread.CurrentThread, "target", arr as JToken);
                }
            }
            return result;
        }
    }
}
