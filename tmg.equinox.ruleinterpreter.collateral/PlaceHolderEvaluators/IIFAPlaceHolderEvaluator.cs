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
    public class IIFAPlaceHolderFunctionEvaluator : IPlaceHolderFunctionEvaluator
    {
        string _placeHolder;
        JToken _dataSource;
        Regex _sqBracketRegex;
        Dictionary<string, JToken> _sources;

        public IIFAPlaceHolderFunctionEvaluator(string placeHolder, JToken dataSource, Dictionary<string, JToken> sources)
        {
            _placeHolder = placeHolder;
            _dataSource = dataSource;
            _sqBracketRegex = new Regex(@"\[[\w\W]*]$");
            _sources = sources;
        }

        public string Evaluate()
        {
            string result = "";
            Match input = _sqBracketRegex.Match(_placeHolder);
            int commaIndex = input.Value.IndexOf(',');
            string condition = input.Value.Substring(0, commaIndex);
            condition = HttpUtility.HtmlDecode(condition);
            condition = condition.Trim(',');
            string richText = input.Value.Substring(commaIndex, input.Value.Length - commaIndex);
            IReportExpressionResolver resolver = new IIFAExpressionResolver();
            ExpressionTemplate template;
            if (condition.Contains("@"))
            {
                RepeaterTemplateResolver rptPlaceHolderEvaluator = new RepeaterTemplateResolver(condition, _sources);
                template = rptPlaceHolderEvaluator.GetRepeaterExpressionTemplate();
            }
            else
            {
                template = resolver.GetExpressionTemplate(condition);
            }
            ExpressionParser parser = new ExpressionParser(null);
            DocumentAlias varb = new DocumentAlias();

            bool isValid = false;
            int matchCount = 0;
            foreach (var filterVarb in template.Variables)
            {

                varb = filterVarb;
                if (!string.IsNullOrEmpty(varb.DocumentDesignName))
                {
                    varb = GetActualValues(filterVarb);
                    matchCount++;
                }

                else if (varb.Alias != null && _dataSource[varb.Alias] != null)
                {
                    varb.ActualValue = _dataSource[varb.Alias].ToString();
                    varb.ActualValueType = ValueType.Single;
                    matchCount++;
                }
                else if (string.IsNullOrEmpty(varb.DocumentDesignName) && string.IsNullOrEmpty(varb.Alias))
                {
                    matchCount++;
                }
            }
            if (matchCount == template.Variables.Count)
            {
                isValid = true;
            }
            bool isMatch = false;
            if (isValid == true)
            {
                isMatch = parser.EvaluateMatch(template);
            }
            if (isMatch == true)
            {
                result = richText;
            }
            return result;
        }


        private DocumentAlias GetActualValues(DocumentAlias filterVar)
        {
            var sourceAliases = _sources["DesignAliases_" + filterVar.DocumentDesignName];
            string path = "";
            var al = sourceAliases.Where(a => a["Alias"].ToString() == filterVar.Alias);
            if (al != null && al.Count() > 0)
            {
                var firstAl = al.First();
                path = firstAl["ElementPath"].ToString();
            }

            if (!String.IsNullOrEmpty(path))
            {
                var tok = _sources[filterVar.DocumentDesignName].SelectToken(path);
                if (tok is JArray)
                {
                    filterVar.ActualValues = GetValues(tok.ToString());
                    filterVar.ActualValueType = ValueType.Array;
                }
                else
                {
                    filterVar.ActualValue = tok != null ? tok.ToString() : string.Empty;
                }

            }
            return filterVar;
        }
        private string[] GetValues(string value)
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
    }
}
