using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Core;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Helper;
using tmg.equinox.ruleinterpreter.operatorutility;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Extension
{
    class AppendArrayFunction : ParserFunction
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
                Parser.Result sync = Utils.GetItem(data, ref from);
                Parser.Result colMaps = Utils.GetItem(data, ref from);


                if (newValue.Token != null && newValue.Token.Count() > 0)
                {
                    JToken targetVal = SourceManager.Get(Thread.CurrentThread, "target");
                    if (sync.String == "true")
                    {
                        if (targetVal != null && targetVal.HasValues)
                        {
                            newValue.Token = JsonParserHelper.MergeArray(targetVal.First(), newValue.Token as JArray, sync.String);
                        }
                    }
                    if (sync.String == "false" && !String.IsNullOrEmpty(colMaps.String))
                    {
                        if (targetVal != null && targetVal.HasValues)
                        {
                            newValue.Token = JsonParserHelper.MergeArray(targetVal.First(), newValue.Token as JArray, sync.String,colMaps.String);
                        }
                    }
                    var settings = new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Concat };
                    JArray exArray = targetVal as JArray;
                    exArray.Merge(newValue.Token, settings);

                    SourceManager.Set(Thread.CurrentThread, "target", exArray as JToken);
                }
            }

            return result;
        }
    }
}
