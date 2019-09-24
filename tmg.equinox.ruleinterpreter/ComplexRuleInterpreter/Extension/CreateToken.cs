using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Helper;
using tmg.equinox.ruleinterpreter.operatorutility;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Extension
{
    class CreateTokenFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result result = new Parser.Result();
            Parser.Result defaultRow = Utils.GetItem(data, ref from);
            JArray arr = new JArray();
            string jsonString = defaultRow.Token != null ? defaultRow.Token.ToString() : defaultRow.String;

            if (jsonString.StartsWith("[") && jsonString.EndsWith("]"))
            {
                result.Token = JArray.Parse(jsonString);
            }
            else
            {
                arr.Add(JToken.Parse(jsonString));
                result.Token = arr as JToken;
            }
            return result;
        }
    }
}
