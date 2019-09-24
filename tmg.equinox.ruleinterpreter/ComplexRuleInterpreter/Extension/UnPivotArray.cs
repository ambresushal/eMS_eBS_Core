using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Helper;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Extension

{
    public class UnPivotArrayFunction : ParserFunction
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
            Parser.Result sourceValue = func.GetValue(data, ref from);

            // 3. Get the child column
            Parser.Result childColumn = Utils.GetItem(data, ref from);
            string columnToUnpivot = FunctionHelper.GetArgument(childColumn);
            JToken sourceOne = sourceValue.Token;
            if (sourceOne != null)
            {
                result.Token = JsonParserHelper.UnPivotArray(sourceOne, columnToUnpivot);
            }
            else
            {
                result.Token = new JArray();
            }
            return result;
        }
    }
}