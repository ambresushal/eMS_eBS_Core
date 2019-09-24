using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class StringPlaceHolderResolver
    {
        string _richText;
        Dictionary<string, JToken> _sources;
        List<JToken> _internalSource;
        public StringPlaceHolderResolver(string richText, Dictionary<string,JToken> sources,List<JToken> internalSource)
        {
            _richText = richText;
            _sources = sources;
            _internalSource = internalSource;
        }

        public string ResolvePlaceHolders()
        {
            //resolve for internal placeholders
            IPlaceHolderEvaluator evaluator = new InternalPlaceHolderEvaluator(_richText, _internalSource);
            //resolve for aliases
            //resolve for fields
            return "";
        }
    }
}
