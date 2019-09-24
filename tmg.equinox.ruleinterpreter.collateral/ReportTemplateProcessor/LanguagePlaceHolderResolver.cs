using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class LanguagePlaceHolderResolver
    {
        string _placeHolder;
        Dictionary<string, JToken> _sources;
        ReportTemplateRowSource _languageSource;
        List<JToken> _contextRows;
        string regexHTMLComment = @"<!--EXTRACT-->";

        public LanguagePlaceHolderResolver(ReportTemplateRowSource languageSource, Dictionary<string, JToken> sources, List<JToken> contextRows)
        {
            _sources = sources;
            _languageSource = languageSource;
            _placeHolder = languageSource.SourceName;
            _contextRows = contextRows;
        }

        public string Process()
        {
            string languageRichText = "";

            if (_languageSource.Source != null && _languageSource.Source.Count() > 0)
            {
                List<JToken> languageRows = GetIntersectList(_languageSource.Source.ToList(), _contextRows);

                if (languageRows.Count > 0 && languageRows[0]["Key"] != null)
                {
                    ExpressionParser parser = new ExpressionParser(_sources);
                    List<ExpressionTemplate> templates = parser.ParseExpressions(languageRows, "");
                    templates = templates.Where(a => !a.TemplateExp.StartsWith("STATIC") && !a.TemplateExp.StartsWith("CUSTOM")).ToList();
                    List<string> matches = parser.EvaluateExpressions(templates);
                    languageRows = languageRows.Where(a => matches.Contains(a["Key"].ToString()) || a["Key"].ToString().StartsWith("CUSTOM") || a["Key"].ToString().StartsWith("STATIC")).ToList();
                }
                languageRows = languageRows.OrderBy(a => a, new SequenceComparer()).ToList();
                foreach (var languageRow in languageRows)
                {
                    if (languageRow["RichText"] != null)
                    {
                        string intersectionFlag = languageRow["SwitchContext"] != null ? languageRow["SwitchContext"].ToString() : string.Empty;
                        List<JToken> currentLangRows = new List<JToken>();
                        currentLangRows.Add(languageRow);
                        List<JToken> currentContext = new List<JToken>();

                        if (intersectionFlag == "Yes")
                        {
                            currentContext = GetIntersectList(currentLangRows, _contextRows);
                        }
                        else
                        {
                            currentContext = GetIntersectList(_contextRows, currentLangRows);
                        }

                        ReportDataResolver thisYearResolver = new ReportDataResolver(JToken.FromObject(currentContext), _sources);
                        currentContext = thisYearResolver.ResolveData().ToList();
                        string richText = languageRow["RichText"].ToString();
                        PlaceHolderProcessor processor = new PlaceHolderProcessor(richText, currentContext, _sources);
                        richText = processor.Process();
                        richText = ExtractSnippet(richText);
                        languageRichText = languageRichText + richText;
                    }
                }
            }
            return languageRichText;
        }

        private List<JToken> GetIntersectList(List<JToken> source, List<JToken> rows)
        {
            List<string> languageCols = GetColumns(source.First());
            List<string> contextCols = GetColumns(rows.First());
            List<string> keys = languageCols.Intersect(contextCols, StringComparer.OrdinalIgnoreCase).Where(a => a != "SequenceNo" && a != "RowIDProperty" && a != "Key" && a != "FilterExpression").ToList();
            var intersect = source.Intersect(rows, new JTokenListEqualityComparer(keys)).ToList();
            List<JToken> results = new List<JToken>();
            int keyCount = keys.Count;
            foreach (var sourceToken in source)
            {
                foreach (var token in intersect)
                {
                    int matchCount = 0;
                    foreach (var key in keys)
                    {
                        if (sourceToken[key].ToString().Trim() == token[key].ToString().Trim())
                        {
                            matchCount++;
                        }
                    }
                    if (keyCount == matchCount)
                    {
                        results.Add(sourceToken);
                    }
                }
            }
            return results;
        }

        private List<string> GetColumns(JToken token)
        {
            List<string> cols = new List<string>();
            foreach (var tok in token.Children())
            {
                string colName = ((JProperty)tok).Name;
                if (colName != "RowIDProperty")
                {
                    cols.Add(colName);
                }
            }
            return cols;
        }

        private string ExtractSnippet(string richText)
        {
            string result = richText;
            var matches = Regex.Matches(richText, regexHTMLComment);
            if(matches.Count == 2)
            {
                List<Capture> captures = new List<Capture>();
                foreach (var match in matches)
                {
                    captures.Add(((Match)match).Captures[0]);
                }
                    var cap = captures.OrderBy(a => a.Index).ToList();
                //generate the primaryJSON search
                int startIndex = cap[0].Index + cap[0].Length;
                int endIndex = cap[1].Index;
                result = richText.Substring(startIndex, endIndex - startIndex);
            }
            return result;
        }
    }
}
