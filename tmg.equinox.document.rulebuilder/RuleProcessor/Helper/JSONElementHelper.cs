using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.model;

namespace tmg.equinox.document.rulebuilder.ruleProcessor.jsonutility
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
