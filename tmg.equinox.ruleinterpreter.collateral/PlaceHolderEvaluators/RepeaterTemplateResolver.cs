using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;


namespace tmg.equinox.ruleinterpretercollateral
{
    public class RepeaterTemplateResolver
    {
        public Dictionary<string, JToken> _sources;
        string _condation = string.Empty;
        string[] aliasCodation;
        List<JToken> tokenDataList = new List<JToken>();
        public RepeaterTemplateResolver(string condation, Dictionary<string, JToken> sources)
        {
            _sources = sources;
            _condation = condation;
        }

        public ExpressionTemplate GetRepeaterExpressionTemplate()
        {
            ExpressionTemplate template = new ExpressionTemplate();
            string expression = string.Empty;
            _condation = RemoveBricket();
            aliasCodation = _condation.Split('@');
            if (aliasCodation.Length > 0)
            {
                expression = aliasCodation[1];
                AliasResolver();
                IReportExpressionResolver resolver = new IIFAExpressionResolver();
                template = resolver.GetExpressionTemplate("[" + expression);
                template.TemplateExp = _condation + "]";
                foreach (var exp in template.Variables)
                {
                    if (!string.IsNullOrEmpty(exp.Operator))
                    {
                        if (tokenDataList.Count() > 0)
                        {
                            tokenDataList = FilterData(exp);
                        }
                    }
                }
                template = SetValue(template);
            }
            return template;
        }

        private void AliasResolver()
        {
            if (aliasCodation.Length > 0)
            {
                string[] alias = aliasCodation[0].Split('[');
                if (alias.Length > 0)
                {
                    string DocumentName = alias.FirstOrDefault();
                    string path = alias.LastOrDefault();
                    if (!string.IsNullOrEmpty(DocumentName) && !string.IsNullOrEmpty(path))
                    {
                        if (_sources.ContainsKey(DocumentName))
                        {
                            tokenDataList = _sources[DocumentName]
                                        .SelectToken(path)
                                        .ToList();
                        }
                    }
                }
            }
        }

        private List<JToken> FilterData(DocumentAlias exp)
        {
            switch (exp.Operator)
            {
                case "=":
                    tokenDataList = (from r in tokenDataList
                                     where (string)r[exp.ActualValue] == exp.Value
                                     select r).ToList();
                    break;
                case "^":
                    tokenDataList = (from r in tokenDataList
                                     where r[exp.ActualValue].ToString().Contains(exp.Value) == true
                                     select r)
                            .ToList();
                    break;
                case "!=":
                    tokenDataList = (from r in tokenDataList
                                     where (string)r[exp.ActualValue] != exp.Value
                                     select r).ToList();
                    break;
                case "Ɐ":
                    string[] sourceval = tokenDataList.Select(sel => (string)sel[exp.ActualValue]).ToArray();
                    string[] myList;
                    myList = new string[] { exp.Value.ToString() };
                    sourceval = sourceval.Intersect(myList).ToArray();

                    if (sourceval.Length > 0)
                    {
                        tokenDataList = (from r in tokenDataList
                                         where r[exp.ActualValue].ToString() == exp.Value.ToString()
                                         select r).ToList();
                    }
                    else
                    {
                        new List<JToken>();
                    }

                    break;
                case "!Ɐ":
                    string[] sourceVal = tokenDataList.Select(sel => (string)sel[exp.ActualValue]).ToArray();
                    string[] dataList = new string[] { exp.Value.ToString() };
                    sourceval = sourceVal.Intersect(dataList).ToArray();

                    if (sourceval.Length == 0)
                    {
                        tokenDataList = new List<JToken>();
                    }
                    break;

            }
            return tokenDataList;
        }

        private string RemoveBricket()
        {
            if (!string.IsNullOrEmpty(_condation))
            {
                if (_condation.StartsWith("["))
                {
                    _condation = _condation.Substring(1, _condation.Length - 1);
                }
                if (_condation.EndsWith("]"))
                {
                    _condation = _condation.Replace("]", "");
                }
            }
            return _condation;
        }

        private ExpressionTemplate SetValue(ExpressionTemplate template)
        {
            if (tokenDataList != null && tokenDataList.Count() > 0)
            {
                foreach (var item in template.Variables)
                {
                    item.ActualValue = (from r in tokenDataList
                                        select (string)r[item.ActualValue])
                                       .FirstOrDefault();
                }
                template.IsValid = true;
            }
            return template;
        }
    }
}
