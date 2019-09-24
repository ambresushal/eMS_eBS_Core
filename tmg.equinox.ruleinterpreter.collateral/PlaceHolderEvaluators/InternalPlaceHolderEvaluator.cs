using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class InternalPlaceHolderEvaluator : IPlaceHolderEvaluator
    {
        string _richText;
        List<JToken> _context;
        public InternalPlaceHolderEvaluator(string richText, List<JToken> context)
        {
            _richText = richText;
            _context = context;
        }
        public string Evaluate()
        {
            string patternInternal = RegexConstants.InternalRegex;
            string richText = _richText;
            MatchCollection collInternal = Regex.Matches(richText, patternInternal);
            //alias matches
            List<string> inserts = new List<string>();
            for (int idx = 0; idx < collInternal.Count; idx++)
            {
                string val = collInternal[idx].Value;
                if (!inserts.Contains(val))
                {
                    inserts.Add(val);
                }
            }
            foreach (var insert in inserts)
            {
                string replace = GetValue(insert);
                richText = richText.Replace(insert, replace);
            }
            return richText;
        }

        private string GetValue(string insert)
        {
            string result = "";

            string replace = insert.Trim(new char[] { '{', '}' });
            if (_context != null)
            {
                var token = _context.First();
                if (token != null)
                {
                    replace = replace.Trim(':');
                    var tok = token.SelectToken(replace);

                    if (tok != null)
                    {
                        result = tok.ToString();
                    }
                }
            }
            return result;
        }
    }
}
