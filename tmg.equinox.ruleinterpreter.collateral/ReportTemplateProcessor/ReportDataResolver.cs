using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class ReportDataResolver
    {
        private JToken _sourceToResolve;
        private Dictionary<string, JToken> _sources;
        private Regex _regex;

        public ReportDataResolver(JToken sourceToResolve, Dictionary<string, JToken> sources)
        {
            _sourceToResolve = sourceToResolve.DeepClone();
            _sources = sources;
            _regex = new Regex(@"[a-zA-Z0-9]*\[[\w.@><=!\s/\*\&\^\-\;\(\)\{\}:]*\]");
        }

        public JToken ResolveData()
        {
            if (_sourceToResolve != null && _sourceToResolve.Count() > 0)
            {
                foreach (var row in _sourceToResolve)
                {
                    foreach (var col in row.Children())
                    {
                        if (((JProperty)col).Name != "LanguageRichText")
                        {
                            string colVal = ((JProperty)col).Value.ToString();
                            List<JToken> context = new List<JToken>();
                            context.Add(row);
                            //InternalPlaceHolderEvaluator evaluator = new InternalPlaceHolderEvaluator(colVal, context);
                            //colVal = evaluator.Evaluate();
                            Match match = _regex.Match(colVal);
                            if (match.Success == true && !colVal.StartsWith("["))
                            {
                                if (((JProperty)col).Name != "Key")
                                {
                                    ResolveValue(col, colVal,context);
                                }
                                else
                                {
                                    InternalPlaceHolderEvaluator evaluator = new InternalPlaceHolderEvaluator(colVal, context);
                                    colVal = evaluator.Evaluate();
                                    ((JProperty)col).Value = colVal;
                                }
                            }
                        }
                    }
                }
            }
            return _sourceToResolve;
        }

        private void ResolveValue(JToken token, string path,List<JToken> context)
        {

            var prop = token as JProperty;
            string[] parts = path.Split('[');
            string source = parts[0];
            bool resolved = false;
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
                                //resolve the Variable values
                                foreach(var variab in template.Variables)
                                {
                                    InternalPlaceHolderEvaluator evaluator = new InternalPlaceHolderEvaluator(variab.Value, context);
                                    variab.Value = evaluator.Evaluate();
                                }
                                int idx = arrayParts[0].LastIndexOf('.');
                                string jsonPathParent = arrayParts[0].Substring(0, idx);
                                string column = arrayParts[0].Substring(idx + 1, arrayParts[0].Length - idx - 1);
                                JToken document = _sources[source];
                                if (!jsonPathParent.Contains(' '))
                                {
                                    var parentToken = document.SelectToken(jsonPathParent, false);
                                    if (parentToken != null && parentToken is JArray)
                                    {
                                        foreach (var tok in parentToken)
                                        {
                                            if (tok.HasValues && tok != null && tok[column] != null)
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
                                                        switch (varb.Operator)
                                                        {
                                                            case "=":
                                                                if (tok[varb.Alias].ToString() == varb.Value)
                                                                {
                                                                    matchCount++;
                                                                }
                                                                break;
                                                            case "Ɐ":
                                                                {
                                                                    string[] values = tok[varb.Alias] is JArray ? GetValuesFromArray(tok[varb.Alias].ToString())
                                                                                      : (tok[varb.Alias].Type == JTokenType.String ? GetValuesFromString(tok[varb.Alias].ToString())
                                                                                          : null
                                                                                         );
                                                                    values = values.Where(a => !String.IsNullOrEmpty(a)).ToArray();
                                                                    if (values.Contains(varb.Value) == true)
                                                                    {
                                                                        matchCount++;
                                                                    }

                                                                    break;
                                                                }
                                                            case "^":
                                                                {
                                                                    if (tok[varb.Alias].ToString().Contains(varb.Value))
                                                                    {
                                                                        matchCount++;
                                                                    }

                                                                    break;
                                                                }
                                                        }
                                                    }
                                                    if (matchCount == template.Variables.Count)
                                                    {
                                                        prop.Value = tok[column].ToString();
                                                        resolved = true;
                                                    }
                                                }
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
                        if (!jsonPath.Contains(' '))
                        {
                            var valToken = document.SelectToken(jsonPath, false);
                            if (valToken != null)
                            {
                                prop.Value = valToken.ToString();
                                resolved = true;
                            }
                        }
                    }

                }
                if (resolved == false)
                {
                    prop.Value = "";
                }
            }
        }

        private string[] GetValuesFromArray(string value)
        {
            string[] values = null;
            value = value.Trim(new char[] { '[', '\r', '\n', ']' });
            value = value.Replace("\"", "");
            values = value.Split(',');
            for (int idx = 0; idx < values.Length; idx++)
            {
                values[idx] = values[idx].Trim();
            }
            return values;
        }

        private string[] GetValuesFromString(string value)
        {
            string[] values = null;
            values = value.Split(new char[] { ';', ',' });
            for (int idx = 0; idx < values.Length; idx++)
            {
                values[idx] = values[idx].Trim();
            }
            return values;
        }
    }
}
