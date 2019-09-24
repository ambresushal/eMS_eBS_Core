using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class ExpressionParser
    {

        Dictionary<string, JToken> _sources;
        Regex _numericRegex;
        Regex _extractDigitRegex;
        public ExpressionParser(Dictionary<string, JToken> sources)
        {
            _sources = sources;
            _numericRegex = new Regex(@"^[\$]{0,1}\d*[,]{0,1}\d*[.]{0,1}\d*[%]{0,1}$");
            _extractDigitRegex = new Regex(@"[^\d.]");
        }
        public List<ExpressionTemplate> ParseExpressions(JToken masterListSource, string filter)
        {
            List<ExpressionTemplate> templates = new List<ExpressionTemplate>();
            IReportExpressionResolver resolver = new ReportExpressionResolver();

            foreach (var token in masterListSource)
            {
                if (String.IsNullOrEmpty(filter) || token["MasterListName"].ToString() == filter)
                {
                    ExpressionTemplate template = resolver.GetExpressionTemplate(token["Key"].ToString());

                    if (token["Key"].ToString().Contains("@") && token["Key"].ToString() != "STATIC")
                    {
                        RepeaterTemplateResolver rptResolver = new RepeaterTemplateResolver(token["Key"].ToString(), _sources);
                        template = rptResolver.GetRepeaterExpressionTemplate();
                    }

                    var templ = templates.Where(t => t.TemplateExp == template.TemplateExp);
                    if (templ == null || templ.Count() == 0)
                    {
                        templates.Add(template);
                    }

                }
            }
            return templates;
        }
        public List<ExpressionTemplate> ParseExpressions(List<JToken> sourceTokens, string filter)
        {
            List<ExpressionTemplate> templates = new List<ExpressionTemplate>();
            IReportExpressionResolver resolver = new ReportExpressionResolver();
            if (sourceTokens.Count > 0 && sourceTokens[0]["Key"] != null)
            {
                foreach (var token in sourceTokens)
                {
                    if (String.IsNullOrEmpty(filter) || token["MasterListName"].ToString() == filter)
                    {
                        ExpressionTemplate template = resolver.GetExpressionTemplate(token["Key"].ToString());
                        if (token["Key"].ToString().Contains("@") && token["Key"].ToString() != "STATIC")
                        {
                            RepeaterTemplateResolver rptResolver = new RepeaterTemplateResolver(token["Key"].ToString(), _sources);
                            template = rptResolver.GetRepeaterExpressionTemplate();
                        }
                        var templ = templates.Where(t => t.TemplateExp == template.TemplateExp);
                        if (templ == null || templ.Count() == 0)
                        {
                            templates.Add(template);
                        }

                    }
                }
            }
            return templates;
        }
        public List<string> EvaluateExpressions(List<ExpressionTemplate> templates)
        {
            List<string> expressions = new List<string>();
            foreach (var templ in templates)
            {
                string expression = templ.Template;
                foreach (var varb in templ.Variables)
                {
                    string designName = varb.DocumentDesignName;
                    if (!string.IsNullOrEmpty(designName) && !string.IsNullOrEmpty(varb.Alias))
                    {
                        //get source
                        JToken source = null;
                        JToken sourceAliases = null;

                        source = _sources[designName];
                        //get source alias mappings
                        sourceAliases = _sources["DesignAliases_" + designName];

                        //get alias value
                        bool isArray = false;
                        varb.ActualValue = GetAliasValue(source, sourceAliases, varb.Alias, false, ref isArray);
                        if (isArray == true)
                        {
                            varb.ActualValueType = ValueType.Array;
                            varb.ActualValues = GetValues(varb.ActualValue);
                        }
                        else
                        {
                            varb.ActualValueType = ValueType.Single;
                        }
                    }
                }
                if (EvaluateMatch(templ) == true)
                {
                    expressions.Add(templ.TemplateExp);
                }
            }
            return expressions;
        }

        public bool EvaluateMatch(ExpressionTemplate template)
        {
            bool result = false;
            int matchCount = 0;
            foreach (DocumentAlias varb in template.Variables)
            {
                switch (varb.Operator)
                {
                    case "=":
                    case "!=":
                    case ">":
                    case ">=":
                    case "<":
                    case "<=":
                    case "^":
                    case "!^":
                        int? intVal = null;
                        double? doubleVal = null;
                        int? intActualVal = null;
                        double? doubleActualVal = null;
                        bool isNumeric = false;
                        bool isActualNumeric = false;
                        isNumeric = TryGetNumeric(varb.Value, ref intVal, ref doubleVal);
                        isActualNumeric = TryGetNumeric(varb.ActualValue, ref intActualVal, ref doubleActualVal);
                        if (isNumeric == true && isActualNumeric == true)
                        {
                            bool mathCompareResult = MathCompare(varb.Operator, intActualVal, doubleActualVal, intVal, doubleVal);
                            if (mathCompareResult == true)
                            {
                                matchCount = matchCount + 1;
                            }
                        }
                        else
                        {
                            bool strCompareResult = StringCompare(varb.Operator, varb.ActualValue, varb.Value);
                            if (strCompareResult == true)
                            {
                                matchCount = matchCount + 1;
                            }
                        }
                        break;
                    case "₳": //ALL
                    case "Ɐ": //ANY
                    case "!Ɐ": //NOT ANY
                        bool mvCompareResult = MultiValueCompare(varb);
                        if (mvCompareResult == true)
                        {
                            matchCount = matchCount + 1;
                        }
                        break;
                    default:
                        break;
                }
            }

            if (template.LogicalOperator == "&" && matchCount == template.Variables.Count)
            {
                result = true;
            }
            if (template.LogicalOperator == "|" && matchCount > 0)
            {
                result = true;
            }
            if (String.IsNullOrEmpty(template.LogicalOperator) && matchCount > 0)
            {
                result = true;
            }
            return result;
        }

        public string GetAliasValue(JToken source, JToken aliases, string alias, bool resolveItem, ref bool isArray)
        {
            string val = "";
            string path = "";

            var al = aliases.Where(a => a["Alias"].ToString() == alias);
            JArray items = null;
            if (al != null && al.Count() > 0)
            {
                var firstAl = al.First();
                path = firstAl["ElementPath"].ToString();
                items = !string.IsNullOrEmpty(firstAl["Items"].ToString()) ? JArray.Parse(firstAl["Items"].ToString()) : null;
            }
            if (!String.IsNullOrEmpty(path))
            {
                var tok = source.SelectToken(path);
                if (tok is JArray)
                {
                    isArray = true;
                }
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

        private bool TryGetNumeric(string input, ref int? intVal, ref double? doubleVal)
        {
            bool isNumeric = false;
            if (!String.IsNullOrEmpty(input))
            {
                Match match = _numericRegex.Match(input);
                if (match.Success == true)
                {
                    double tDoubleVal;
                    int tIntVal;
                    input = _extractDigitRegex.Replace(input, "");
                    if (input.IndexOf('.') > 0)
                    {
                        isNumeric = double.TryParse(input, out tDoubleVal);
                        if (isNumeric == true)
                        {
                            doubleVal = tDoubleVal;
                        }
                    }
                    else
                    {
                        isNumeric = int.TryParse(input, out tIntVal);
                        if (isNumeric == true)
                        {
                            intVal = tIntVal;
                        }
                    }
                }
            }
            return isNumeric;
        }

        private bool MathCompare(string op, int? intActualVal, double? doubleActualVal, int? intVal, double? doubleVal)
        {
            bool result = false;
            double actualValue;
            double value;
            if (intActualVal != null)
            {
                actualValue = intActualVal.Value;
            }
            else
            {
                actualValue = doubleActualVal.Value;

            }
            if (intVal != null)
            {
                value = intVal.Value;
            }
            else
            {
                value = doubleVal.Value;
            }

            switch (op)
            {
                case "=":
                    if (actualValue == value)
                    {
                        result = true;
                    }
                    break;
                case "!=":
                    if (actualValue != value)
                    {
                        result = true;
                    }
                    break;
                case ">":
                    if (actualValue > value)
                    {
                        result = true;
                    }
                    break;
                case ">=":
                    if (actualValue >= value)
                    {
                        result = true;
                    }
                    break;
                case "<":
                    if (actualValue < value)
                    {
                        result = true;
                    }
                    break;
                case "<=":
                    if (actualValue <= value)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool StringCompare(string op, string actualVal, string val)
        {
            bool result = false;
            switch (op)
            {
                case "=":
                    if (String.Compare(actualVal, val, true) == 0)
                    {
                        result = true;
                    }
                    break;
                case "!=":
                    if (String.Compare(actualVal, val, true) != 0)
                    {
                        result = true;
                    }
                    break;
                case ">":
                    if (String.Compare(actualVal, val, true) > 0)
                    {
                        result = true;
                    }
                    break;
                case ">=":
                    if (String.Compare(actualVal, val, true) > -1)
                    {
                        result = true;
                    }
                    break;
                case "<":
                    if (String.Compare(actualVal, val, true) < 0)
                    {
                        result = true;
                    }
                    break;
                case "<=":
                    if (String.Compare(actualVal, val, true) < 1)
                    {
                        result = true;
                    }
                    break;
                case "^":
                    if (actualVal != null)
                    {

                        if (actualVal.IndexOf(val) >= 0)
                        {
                            result = true;
                        }
                    }
                    break;
                case "!^":
                    result = !actualVal.Contains(val);
                    break;
            }
            return result;
        }

        private bool MultiValueCompare(DocumentAlias varb)
        {
            bool result = false;
            if (varb.ValueType != ValueType.Array || varb.ActualValueType != ValueType.Array)
            {
                if (varb.ValueType != ValueType.Array)
                {
                    JArray arr = new JArray();
                    arr.Add(varb.Value);
                    varb.Values = GetValues(arr.ToString());
                }
                if (varb.ActualValueType != ValueType.Array)
                {
                    JArray arr = new JArray();
                    arr.Add(varb.ActualValue);
                    varb.ActualValues = GetValues(arr.ToString());
                }
            }

            string[] results = varb.ActualValues.Intersect(varb.Values).ToArray();
            switch (varb.Operator)
            {
                case "₳": //ALL
                    if (results.Length == varb.ActualValues.Length)
                    {
                        result = true;
                    }
                    break;
                case "Ɐ": //ANY
                    if (results.Length > 0)
                    {
                        result = true;
                    }
                    break;
                case "!Ɐ": //NOT ANY
                    if (results.Length == 0)
                    {
                        result = true;
                    }
                    break;
            }
            return result;

        }

        private string[] GetValues(string value)
        {
            string parentString = value;
            string[] values = null;
            string[] excludeStrings = new string[] { "\\r", "\\n", "\\", "[", "]" };
            value = value.Trim(new char[] { '[', '\r', '\n', ']' });
            value = value.Replace("\"", "");

            foreach (var item in excludeStrings)
            {
                value = value.Replace(item, string.Empty);
            }

            values = value.Split(',', ';');
            for (int idx = 0; idx < values.Length; idx++)
            {
                values[idx] = values[idx].Trim();
            }
            return values;
        }
    }

}
