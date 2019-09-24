using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.model;

namespace tmg.equinox.ruleinterpreter.ruleProcessor.jsonutility
{
    public static class JSONElementHelper
    {
        public static List<JToken> AppendChildren(this List<JToken> jTokens, JObject childRows)
        {
            foreach (JProperty jProperty in childRows.Children())
            {
                jTokens.ForEach(x => x.Last().AddAfterSelf(jProperty));
            }
            return jTokens;
        }
    }
}
