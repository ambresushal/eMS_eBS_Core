using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class DataRowsResolver
    {
        string _placeHolder;
        ReportTemplateRow _row;
        Dictionary<string, JToken> _sources;
        ReportTemplateRowSource _dataSource;
        List<JToken> _contextRows;
        string regexHTMLComment = @"<!--EXTRACT-->";

        public DataRowsResolver(ReportTemplateRow row, ReportTemplateRowSource dataSource, Dictionary<string, JToken> sources, List<JToken> contextRows)
        {
            _row = row;
            _sources = sources;
            _dataSource = dataSource;
            _placeHolder = dataSource.SourceName;
            _contextRows = contextRows;
        }

        public string Process()
        {
            string dataRichText = "";
            if (_dataSource.Source != null && _dataSource.Source.Count() > 0)
            {
                List<JToken> dataRows = GetIntersectList(_dataSource.Source.ToList(), _contextRows);
                if (dataRows.Count > 0 && dataRows[0]["Key"] != null)
                {
                    ExpressionParser parser = new ExpressionParser(_sources);
                    List<ExpressionTemplate> templates = parser.ParseExpressions(dataRows, "");
                    templates = templates.Where(a => !a.TemplateExp.StartsWith("STATIC") && !a.TemplateExp.StartsWith("CUSTOM")).ToList();
                    List<string> matches = parser.EvaluateExpressions(templates);
                    dataRows = dataRows.Where(a => matches.Contains(a["Key"].ToString()) || a["Key"].ToString().StartsWith("CUSTOM") || a["Key"].ToString().StartsWith("STATIC")).ToList();
                }
                dataRows = dataRows.OrderBy(a => a, new SequenceComparer()).ToList();
                string templatePattern = _row.TemplateRowName.Replace("[", @"\[").Replace("]", @"\]").Replace(@"\\", @"\");
                string pattern = @"<!--" + templatePattern + @"-->.*<!--" + templatePattern + @"-->";
                MatchCollection coll = Regex.Matches(_row.RichText, pattern, RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
                string templateStr = "";
                string rowRichText = "";
                if (coll.Count > 0)
                {
                    Match col = coll[0];
                    templateStr = col.ToString();
                    rowRichText = col.ToString().Replace("<!--" + _row.TemplateRowName + "-->", "");
                }
                foreach (var dataRow in dataRows)
                {
                    if (!String.IsNullOrEmpty(_row.RichText))
                    {
                        List<JToken> currentDataRows = new List<JToken>();
                        currentDataRows.Add(dataRow);
                        List<JToken> currentContext = GetIntersectList(currentDataRows, _contextRows);
                        string richText = _row.RichText.ToString();
                        if (!String.IsNullOrEmpty(rowRichText))
                        {
                            richText = rowRichText;
                        }
                        //filter by Filter Column
                        List<JToken> processedRows = PreProcessRows(currentContext);
                        foreach (var procRow in processedRows)
                        {
                            foreach (var prop in procRow.Children())
                            {
                                if (((JProperty)prop).Name == "LanguageRichText")
                                {
                                    List<JToken> token = new List<JToken>();
                                    token.Add(procRow);
                                    PlaceHolderProcessor processor = new PlaceHolderProcessor(procRow["LanguageRichText"].ToString(), token, _sources);
                                    string languageText = processor.Process();
                                    languageText = ExtractSnippet(languageText);
                                    var procProp = procRow["LanguageRichText"] as JProperty;
                                    ((JProperty)prop).Value = languageText;
                                }
                            }
                        }
                        if (processedRows.Count > 0)
                        {
                            PlaceHolderProcessor processor = new PlaceHolderProcessor(richText, processedRows, _sources);
                            richText = processor.Process();
                            dataRichText = dataRichText + richText;
                        }
                    }
                }
                if (String.IsNullOrEmpty(templateStr) == false && dataRows !=null && dataRows.Count>0)
                {
                    dataRichText = _row.RichText.Replace(templateStr, dataRichText);
                }
            }
            return dataRichText;
        }

        private List<JToken> GetIntersectList(List<JToken> source, List<JToken> rows)
        {
            List<string> dataCols = GetColumns(source.First());
            List<string> contextCols = GetColumns(rows.First());
            List<string> keys = dataCols.Intersect(contextCols, StringComparer.OrdinalIgnoreCase).Where(a => a != "SequenceNo" && a != "RowIDProperty" && a!="Key" && a!="FilterExpression").ToList();
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
                        if (sourceToken[key] != null && token[key] != null)
                        {
                            if (sourceToken[key].ToString().Trim() == token[key].ToString().Trim())
                            {
                                matchCount++;
                            }
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

        private List<JToken> FilterRows(List<JToken> rows)
        {
            List<JToken> filteredRows = new List<JToken>();
            if (_row.Filter != null)
            {
                if (rows != null && rows.Count > 0)
                {
                    JToken result = JToken.Parse("[]");
                    foreach (var varb in _row.Filter.FilterTemplate.Variables)
                    {
                        foreach (var row in rows)
                        {
                            if (row[varb.Alias].ToString() == varb.Value)
                            {
                                filteredRows.Add(row);
                            }
                        }
                    }
                }
            }
            return filteredRows;
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

        private List<JToken> PreProcessRows(List<JToken> rows)
        {
            List<JToken> modifiedRows = new List<JToken>();
            foreach (var row in rows)
            {
                if (row["Filter"] != null)
                {
                    string filter = row["Filter"].ToString();
                    IReportExpressionResolver resolver = new ReportExpressionResolver();
                    ExpressionTemplate template = resolver.GetExpressionTemplate(filter);
                    int matchCount = 0;
                    foreach (var varb in template.Variables)
                    {
                        if (row[varb.Alias] != null)
                        {
                            int intRowVal;
                            int intVarbVal;

                            string val = row[varb.Alias].ToString();
                            switch (varb.Operator)
                            {
                                case "=":
                                    if (val == varb.Value)
                                    {
                                        matchCount = matchCount + 1;
                                    }
                                    break;
                                case "!=":
                                    if (val != varb.Value)
                                    {
                                        matchCount = matchCount + 1;
                                    }
                                    break;

                                case ">":
                                case "<":
                                case ">=":
                                case "<=":
                                    if (int.TryParse(val, out intRowVal) && int.TryParse(varb.Value, out intVarbVal))
                                    {
                                        if (varb.Operator == ">" && intRowVal > intVarbVal)
                                        {
                                            matchCount = matchCount + 1;
                                        }
                                        if (varb.Operator == ">=" && intRowVal >= intVarbVal)
                                        {
                                            matchCount = matchCount + 1;
                                        }
                                        if (varb.Operator == "<" && intRowVal < intVarbVal)
                                        {
                                            matchCount = matchCount + 1;
                                        }
                                        if (varb.Operator == "<=" && intRowVal <= intVarbVal)
                                        {
                                            matchCount = matchCount + 1;
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    if (matchCount == template.Variables.Count)
                    {
                        //modify for Copay
                        modifiedRows.Add(row);
                    }
                }

                else
                {
                    modifiedRows.Add(row);
                }
            }

            return modifiedRows;
        }

        private string GetValue(JToken row, string column)
        {
            string result = "";
            if (row != null)
            {
                var token = row;
                if (token != null)
                {
                    var tok = token.SelectToken(column);
                    if (tok != null)
                    {
                        result = tok.ToString();
                    }
                }
            }
            return result;
        }
        private string ExtractSnippet(string richText)
        {
            string result = richText;
            var matches = Regex.Matches(richText, regexHTMLComment);
            if (matches.Count == 2)
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
