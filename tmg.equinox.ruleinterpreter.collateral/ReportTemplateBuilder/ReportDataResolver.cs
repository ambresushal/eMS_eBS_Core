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
            _regex = new Regex(@"\w*\[[\w\.@=><!]*\]");
        }

        public JToken ResolveData()
        {
            if (_sourceToResolve != null && _sourceToResolve.Count() > 0)
            {
                foreach (var row in _sourceToResolve)
                {
                    foreach (var col in row.Children())
                    {
                        if (((JProperty)col).Name != "Key")
                        {
                            string colVal = ((JProperty)col).Value.ToString();
                            Match match = _regex.Match(colVal);
                            if (match.Success == true && !colVal.StartsWith("["))
                            {
                                ResolveValue(col, colVal);
                            }
                        }
                    }
                }
            }
            return _sourceToResolve;
        }

        private void ResolveValue(JToken token, string path)
        {
            var prop = token as JProperty;
            string[] parts = path.Split('[');
            string source = parts[0];
            bool resolved = false;
            if(parts.Length > 1)
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
                                if (parentToken != null && parentToken is JArray)
                                {
                                    foreach (var tok in parentToken)
                                    {
                                        if (tok[column] != null)
                                        {
                                            int matchCount = 0;
                                            foreach (var varb in template.Variables)
                                            {
                                                if(String.IsNullOrEmpty(varb.Alias))
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
                                                prop.Value = tok[column].ToString();
                                                resolved = true;
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
                        var valToken = document.SelectToken(jsonPath);
                        //SJ: resolve for Array

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
}
