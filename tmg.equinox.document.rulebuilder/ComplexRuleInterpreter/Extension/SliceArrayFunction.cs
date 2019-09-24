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
    class SliceArrayFunction : ParserFunction
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
                JArray removeRows = newValue.Token as JArray;

                // 4. Get the keys by comma separated
                Parser.Result keyColumns = Utils.GetItem(data, ref from);


                if (removeRows != null && removeRows.Count > 0)
                {
                    JToken targetVal = SourceManager.Get(Thread.CurrentThread, "target");
                    JArray sourceArray = targetVal as JArray;
                    List<string> keys = keyColumns.String.Split(',').ToList();

                    if (keys.Count > 0)
                    {
                        List<JToken> common = sourceArray.Intersect(removeRows, new ObjectEqualityComparer(keys)).ToList();
                        foreach (var row in common)
                        {
                            sourceArray.Remove(row);
                        }
                        IEnumerable<JToken> res = sourceArray.Distinct(new ObjectEqualityComparer(keys)).ToList();
                        JArray arr = new JArray();
                        foreach (var tok in res)
                        {
                            arr.Add(tok);
                        }
                        sourceArray = arr;
                        SourceManager.Set(Thread.CurrentThread, "target", sourceArray as JToken);
                    }
                }
            }

            return result;
        }
    }
}
