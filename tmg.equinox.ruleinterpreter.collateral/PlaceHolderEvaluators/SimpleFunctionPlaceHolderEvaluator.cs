using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class SimpleFunctionPlaceHolderEvaluator : IPlaceHolderEvaluator
    {
        string _richText;
        List<JToken> _context;
        Dictionary<string, JToken> _sources;
        List<LanguageFormats> _langFormats;
        public SimpleFunctionPlaceHolderEvaluator(string richText, List<JToken> context, Dictionary<string, JToken> sources)
        {
            _richText = richText;
            _context = context;
            _sources = sources;
        }
        public string Evaluate()
        {
            _langFormats = EvaluateLanguageFormats();

            foreach (var langFormat in _langFormats)
            {
                try
                {
                    PlaceHolderParser parser = new PlaceHolderParser(langFormat.FormatString);
                    if (parser.HasInternal())
                    {
                        InternalPlaceHolderEvaluator evaluator = new InternalPlaceHolderEvaluator(langFormat.FormatString, _context);
                        langFormat.FormatString = evaluator.Evaluate();
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                }
            }

            string patternFunction = RegexConstants.SimpleFunctionRegex;
            string richText = _richText;
            List<Capture> captures = new List<Capture>();
            MatchCollection collFunction = Regex.Matches(richText, patternFunction);
            List<string> inserts = new List<string>();
            for (int idx = 0; idx < collFunction.Count; idx++)
            {
                string val = collFunction[idx].Value;
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
            result = EvaluatePlaceHolderFunction(replace);
            return result;
        }

        private string EvaluatePlaceHolderFunction(string expression)
        {
            string result = "";
            SimpleFunctionEvaluatorFactory factory = new SimpleFunctionEvaluatorFactory();
            JToken token = null;
            if(_context != null)
            {
                token = _context.First();
            }
            IPlaceHolderFunctionEvaluator evaluator = factory.GetInstance(expression, token, _sources, _langFormats);
            result = evaluator.Evaluate();
            return result;
        }

        private List<LanguageFormats> EvaluateLanguageFormats()
        {
            string formatType = string.Empty;
            List<LanguageFormats> langFormats = new List<LanguageFormats>();
            JToken languageFormatSource = _sources.Where(whr => whr.Key.Contains("FORMAT")).Select(sel => sel.Value).FirstOrDefault();

            if (languageFormatSource != null && languageFormatSource.Count() > 0)
            {
                formatType = languageFormatSource.First().SelectToken(LanguageFormatConstants.FormTypeTokenPath).ToString();
            }
            if (!string.IsNullOrEmpty(formatType))
            {
                LanguageFormatParser parser = new LanguageFormatParser(languageFormatSource);
                langFormats = parser.GetLanguageFormats();
            }
            return langFormats;
        }

    }
}
