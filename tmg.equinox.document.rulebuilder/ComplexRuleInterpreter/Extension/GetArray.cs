using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Core;
using tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Helper;

namespace tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Extension
{
    class GetArrayFunction : ParserFunction
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

            // 3. Get the value of Filter
            Parser.Result columnName = Utils.GetItem(data, ref from);
            string propName = columnName.String;

            if (sourceValue.Token != null && sourceValue.Token.HasValues)
            {
                if (sourceValue.Token.Type == JTokenType.Array) { result.Token = sourceValue.Token[0][propName]; }
                else if (sourceValue.Token[propName] != null && sourceValue.Token[propName].Type == JTokenType.Array)
                {
                    result.Token = sourceValue.Token[propName];
                }
                else { result.String = Convert.ToString(sourceValue.Token[propName]); }
            }
            else
            {
                result.String = string.Empty;
            }
            return result;
        }
    }
}
