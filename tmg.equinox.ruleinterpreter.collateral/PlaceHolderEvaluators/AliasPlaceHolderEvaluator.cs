using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class AliasPlaceHolderEvaluator : IPlaceHolderEvaluator
    {
        string _richText;
        Dictionary<string, JToken> _sources;
        public AliasPlaceHolderEvaluator(string richText,Dictionary<string,JToken> sources)
        {
            _richText = richText;
            _sources = sources;
        }
        public string Evaluate()
        {
            string patternAliases = RegexConstants.AliasRegex;
            string richText = _richText;
            //alias matches
            MatchCollection coll = Regex.Matches(richText, patternAliases);
            List<string> inserts = new List<string>();
            foreach (var col in coll)
            {
                string val = col.ToString();
                if (!inserts.Contains(val))
                {
                    inserts.Add(val);
                }
            }
            foreach (var insert in inserts)
            {
                string replace = GetValueForAlias(insert);
                richText = richText.Replace(insert, replace);
            }
            return richText;
        }

        private string GetValueForAlias(string insert)
        {
            string result = "";
            string replace = insert.Trim(new char[] { '{', '}', ']' });
            string[] parts = replace.Split('[');
            if (parts.Length > 1)
            {
                string designName = parts[0];
                string alias = parts[1].TrimEnd('[');
                var source = _sources[designName];
                //get source alias mappings
                var sourceAliases = _sources["DesignAliases_" + designName];
                result = GetAliasValue(source, sourceAliases, alias, true);
            }
            return result;
        }

        private string GetAliasValue(JToken source, JToken aliases, string alias, bool resolveItem)
        {
            string val = "";
            string path = "";
            var al = aliases.Where(a => a["Alias"].ToString() == alias);
            JArray items = null;
            if (al != null && al.Count() > 0)
            {
                var firstAl = al.First();
                path = firstAl["ElementPath"].ToString();
                items = JArray.Parse(firstAl["Items"].ToString());
            }
            if (!String.IsNullOrEmpty(path))
            {
                val = source.SelectToken(path).ToString();
                if (resolveItem == true)
                {
                    if (items != null && items.Count > 0)
                    {
                        var matchItems = items.Where(a => a["Value"].ToString() == val).Select(a => a["DisplayText"].ToString());
                        if (matchItems != null && matchItems.Count() > 0)
                        {
                            val = matchItems.First();
                        }
                    }
                }
            }
            return val;
        }
    }
}
