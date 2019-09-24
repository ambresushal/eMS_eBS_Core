using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class FieldPlaceHolderEvaluator : IPlaceHolderEvaluator
    {
        private Dictionary<string, JToken> _sources;
        private string _inputString;
        public FieldPlaceHolderEvaluator(string inputString, Dictionary<string, JToken> sources)
        {
            _inputString = inputString;
            _sources = sources;
        }

        public string Evaluate()
        {
            MatchCollection matches = Regex.Matches(_inputString, RegexConstants.FieldRegex);
            foreach (var item in matches)
            {
                _inputString = _inputString.Replace(item.ToString(), ResolveValue(WebUtility.HtmlDecode(item.ToString())));
            }
            return _inputString;
        }
        private string ResolveValue(string input)
        {
            string resultString = string.Empty;

                input = input.Trim(new char[] { '{', '}', ']' });
                string[] parts = input.Split('[');
                string source = parts[0];
                if (parts.Length > 1)
                {
                    string jsonPath = parts[1].Replace("]", "").Replace("[", "");
                    if (_sources.ContainsKey(source))
                    {
                        //check for array
                        if (jsonPath.Contains("@") == true)
                        {
                            string[] arrayParts = jsonPath.Split('@');
                            if (arrayParts.Length > 1)
                            {
                                string filter = arrayParts[1];
                                IReportExpressionResolver resolver = new ReportExpressionResolver();
                                ExpressionTemplate template = resolver.GetExpressionTemplate(filter);
                                if (template.Variables != null && template.Variables.Count > 0)
                                {
                                    int idx = arrayParts[0].LastIndexOf('.');
                                    string jsonPathParent = arrayParts[0].Substring(0, idx);
                                    string column = arrayParts[0].Substring(idx + 1, arrayParts[0].Length - idx - 1);
                                    JToken document = _sources[source];
                                    var parentToken = document.SelectToken(jsonPathParent);

                                try
                                {
                                    JToken firstRow = parentToken.First();
                                    string childColumnName = string.Empty;
                                    foreach (var tok in firstRow.Children())
                                    {
                                        if (((JProperty)tok).Value.Type == JTokenType.Array)
                                        {
                                            childColumnName = ((JProperty)tok).Name;
                                            break;
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(childColumnName))
                                    {
                                        string sourceAction = "UNPIVOT[" + childColumnName + "]";
                                        SourceActionFormatter formatter = new SourceActionFormatter(null, parentToken, sourceAction);
                                        parentToken = formatter.FormatSource();
                                    }
                                }
                                catch (Exception ex)
                                {

                                }

                                    if (parentToken != null && parentToken is JArray)
                                    {
                                        foreach (var tok in parentToken)
                                        {
                                            if (tok[column] != null)
                                            {
                                                int matchCount = 0;
                                                foreach (var varb in template.Variables)
                                                {
                                                    if (String.IsNullOrEmpty(varb.Alias))
                                                    {
                                                        varb.Alias = varb.DocumentDesignName;
                                                    }
                                                    if (tok[varb.Alias] != null)
                                                    {
                                                        //only Equals (=) operator supported at present
                                                        if (tok[varb.Alias].ToString() == varb.Value)
                                                        {
                                                            matchCount++;
                                                        }
                                                    }
                                                }
                                                if (matchCount == template.Variables.Count)
                                                {
                                                    resultString = tok[column].ToString();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            JToken document = _sources[source];
                            jsonPath = jsonPath.Replace(" ", "");
                            var valToken = document.SelectToken(jsonPath);
                            if (valToken != null)
                            {
                                resultString = valToken.ToString();
                            }
                        }
                    }
            }
            return resultString;
        }
    }
}
