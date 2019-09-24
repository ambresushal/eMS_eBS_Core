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
    class CrossJoinFunction : ParserFunction
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

            // 2. Get the value of source one
            ParserFunction func = ParserFunction.GetFunction(varName);
            Parser.Result sourceOne = func.GetValue(data, ref from);

            // 3. Get the value of source two
            Parser.Result sourceTwo = Utils.GetItem(data, ref from);

            // 4. Filter the source by columns and return the result
            if (sourceOne.Token.Type != JTokenType.Array || sourceOne.Token.Type != JTokenType.Array)
            {
                throw new ArgumentException("invalid source type.");
            }

            JArray sourceToken = sourceOne.Token as JArray;
            JArray targetToken = sourceTwo.Token as JArray;

            JToken crossjoin = JsonParserHelper.CrossJoin(sourceToken, targetToken);
            if (crossjoin != null && crossjoin.Count() > 0)
            {
                result.Token = crossjoin;
            }
            else
            {
                result.Token = new JArray();
            }

            return result;
        }


    }

}
