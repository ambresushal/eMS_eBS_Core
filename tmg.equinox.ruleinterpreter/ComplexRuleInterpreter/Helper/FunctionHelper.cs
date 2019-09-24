using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Core;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Helper;
using tmg.equinox.ruleinterpreter.jsonhelper;


namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Extension
{
    public static class FunctionHelper
    {

        public static Parser.Result GetFilter(Parser.Result currentValue, Parser.Result filterValue)
        {
            Parser.Result filterResult = new Parser.Result();
            if (filterValue.String == null)
            {
                if (currentValue.Token.Count() != 0)
                {
                    filterResult.Token = ((JArray)currentValue.Token)[Convert.ToInt32(filterValue.Value)];
                }
            }
            else if (filterValue.String == string.Empty)
            {
                filterResult.Token = currentValue.Token;
            }
            else
            {
                filterResult.Token = currentValue.Token[filterValue.String];
            }

            return filterResult;
        }

        public static string GetArgument(Parser.Result result)
        {
            string arg = string.Empty;

            if (result.Token != null)
            {
                string[] exclusion = new string[] { "[]", "{}" };
                if (((result.Token.Type == Newtonsoft.Json.Linq.JTokenType.String) || (result.Token.Type == Newtonsoft.Json.Linq.JTokenType.Integer) || (result.Token.Type == Newtonsoft.Json.Linq.JTokenType.Boolean)) && !exclusion.Contains(result.Token.ToString()))
                {
                    arg = result.Token.ToString();
                }
                else if (result.Token.Type == JTokenType.Object)
                {
                    arg = result.Token.ToString();
                }
                else if (result.Token.Type == JTokenType.Array)
                {
                    arg = Newtonsoft.Json.JsonConvert.SerializeObject(result.Token);
                }
            }
            else if (!string.IsNullOrEmpty(result.String))
            {
                arg = result.String;
            }
            else if (!double.IsNaN(result.Value))
            {
                arg = Convert.ToString(result.Value);
            }

            return arg;
        }

        public static int GetCount(Parser.Result result)
        {
            int count = 0;
            if (result.Token != null)
            {
                string[] exclusion = new string[] { "[]", "{}" };
                if (result.Token.Type == Newtonsoft.Json.Linq.JTokenType.String)
                {
                    if (exclusion.Contains(result.Token.ToString()))
                    {
                        count = 0;
                    }
                    else
                    {
                        count = 1;
                    }
                }
                else if (result.Token.Type == Newtonsoft.Json.Linq.JTokenType.Array)
                {
                    count = result.Token.Count();
                    if (count == 1)
                    {
                        if (!IsTokenHasValue(result.Token[0]))
                        {
                            count = 0;
                        }
                    }
                }
            }

            return count;
        }

        public static bool IsTokenHasValue(JToken token)
        {
            bool hasValue = false;
            var properties = token.Children();
            foreach (JProperty prop in properties)
            {
                if (!string.IsNullOrEmpty(prop.Value.ToString()))
                {
                    hasValue = true;
                    break;
                }
            }

            return hasValue;
        }

        public static JToken EvaluteIndex(JToken data, int index, string colList, string childContainer)
        {
            JToken output = null;
            if (index < ((JArray)data).Count)
            {
                JToken t = ((JArray)data)[index];

                if (!string.IsNullOrEmpty(childContainer))
                {
                    t = t[childContainer];
                    output = JToken.FromObject(GetDataByColumn(t.ToList(), colList));
                }
                else
                {
                    output = JToken.FromObject(GetDataByColumn(new List<JToken>() { t }, colList));
                }
            }
            return output;
        }

        public static JToken EvaluteOr(JToken data, Condition rule, string colList)
        {
            JToken output = null;

            foreach (var exp in rule.Expression)
            {
                List<JToken> t = (from r in data where (string)r[exp.Column] == exp.Value select r).ToList();

                if (t.Count > 0)
                {
                    output = JToken.FromObject(GetDataByColumn(t, colList));
                    break;
                }
            }

            return output;
        }

        public static JToken EvaluteAnd(JToken data, Condition rule, string colList)
        {
            List<JToken> t = (from r in data select r).ToList();

            t = t.Where(m => ((JToken) m).HasValues== true).ToList();
            
            var result = new object();

            foreach (var exp in rule.Expression)
            {
                t = (from r in t where (string)r[exp.Column] == exp.Value select r).ToList();
            }

            if (!string.IsNullOrEmpty(colList) && !colList.Contains(',') && t.Count == 1)
            {

                int propertyCount = 0;
                int inputColumnCount = 0;
                string[] columnsToFilter = colList.Split(',');
                inputColumnCount = columnsToFilter.Count();
                JObject keySource = (JObject)t.First();
                foreach (JProperty property in keySource.Properties())
                {
                    if (columnsToFilter.Contains(property.Name))
                    {
                        propertyCount = propertyCount + 1;
                        if (propertyCount == inputColumnCount)
                            break;
                    }
                }

                if (propertyCount == inputColumnCount)
                {
                    result = JToken.FromObject(t[0][colList]);
                }
                else
                {
                    result = string.Empty;
                }
            }
            else
            {
                result = GetDataByColumn(t, colList);
            }

            return JToken.FromObject(result);
        }

        public static JArray GetDataByColumn(List<JToken> data, string columns)
        {
            JArray repeaterRows = new JArray();
            if (!string.IsNullOrEmpty(columns))
            {
                string[] cols = columns.Split(',');
                if (cols.Length > 0)
                {
                    foreach (var row in data)
                    {
                        JObject objRow = new JObject();
                        foreach (var col in row.Children())
                        {
                            if (Array.Exists(cols, e => e.Equals(((JProperty)col).Name)))
                            {
                                objRow.Add(col);
                            }
                        }
                        repeaterRows.Add(objRow as JToken);
                    }
                }
            }
            else
            {
                repeaterRows = JArray.FromObject(data);
            }

            return repeaterRows;
        }

        public static Condition GetCondition(string filter)
        {
            filter = HandleKeyCharacter(filter);


            Condition filterCondition = new Condition() { LogicalOperator = "AND" };
            string[] expressions = filter.Split(new char[] { '&' });

            if (filter.IndexOf("|") >= 0)
            {
                filterCondition.LogicalOperator = "OR";
                expressions = filter.Split(new char[] { '|' });
            }

            filterCondition.Expression = new List<Expression>();
            foreach (var item in expressions)
            {
                filterCondition.Expression.Add(GetExpression(item));
            }

            return filterCondition;
        }

        public static Expression GetExpression(string expression)
        {
            var pattern = @"(?<=\[)(.*?)(?=\])";
            MatchCollection mc = Regex.Matches(expression, pattern);

            Expression expr = new Expression();
            if (mc.Count > 0)
            {
                string colName = expression.Substring(0, expression.IndexOf('='));
                if (colName.Contains('.'))
                {
                    string[] childColumns = colName.Split('.');
                    expr.Container = childColumns[0];
                    expr.Column = childColumns[1];
                }
                else
                {
                    expr.Column = colName;
                }
                expr.Operand = "=";
                expr.Value = Convert.ToString(mc[0]).Replace("\\and", "&").Replace("\\or", "|");
            }
            return expr;
        }

        public static Parser.Result ConvertToType(Parser.Result result, string dataType)
        {
            if (dataType.ToUpper() == "INT")
            {
                result = ConvertToInt(result);
            }
            else if (dataType.ToUpper() == "DOUBLE")
            {
                result = ConvertToDouble(result);
            }
            else if (dataType.ToUpper() == "MATHCEIL")
            {
                result = ConvertToMathCeiling(result);
            }
            return result;
        }


        private static Parser.Result ConvertToInt(Parser.Result result)
        {
            int number = 0;
            if (!string.IsNullOrEmpty(result.String))
            {
                number = Convert.ToInt32(result.String);
            }
            else if (!result.Token.IsNullOrEmpty())
            {
                number = Convert.ToInt32(result.Token);
            }
            else
            {
                number = Convert.ToInt32(result.Value);
            }

            result.String = null;
            result.Token = null;
            result.Value = number;

            return result;
        }


        private static Parser.Result ConvertToDouble(Parser.Result result)
        {
            double number = 0;
            if (!string.IsNullOrEmpty(result.String))
            {
                number = Convert.ToDouble(result.String);
            }
            else if (!result.Token.IsNullOrEmpty())
            {
                number = Convert.ToDouble(result.Token);
            }

            result.String = null;
            result.Token = null;
            result.Value = number;

            return result;
        }

        private static Parser.Result ConvertToMathCeiling(Parser.Result result)
        {
            decimal number = 0;
            if (!string.IsNullOrEmpty(result.String))
            {
                number = Convert.ToDecimal(result.String);
            }
            else if (!result.Token.IsNullOrEmpty())
            {
                number = Convert.ToDecimal(result.Token);
            }

            int roundUpValue = Convert.ToInt32(Math.Ceiling(number));

            result.String = null;
            result.Token = null;
            result.Value = roundUpValue;

            return result;
        }




        private static string HandleKeyCharacter(string expressionString)
        {
            var pattern = @"(?<=\[)(.*?)(?=\])";
            var matchedStrings = Regex.Matches(expressionString, pattern).Cast<Match>().ToList();

            if (matchedStrings.FindAll(x => x.Value.Contains('&') || x.Value.Contains('|')).Count > 0)
            {
                foreach (var matchedItem in matchedStrings)
                {
                    if(!string.IsNullOrEmpty(matchedItem.ToString()))
                    expressionString = expressionString.Replace(matchedItem.ToString(), matchedItem.ToString().Replace("&", "\\and").Replace("|", "\\or"));
                }
            }
            return expressionString;
        }

        public static JToken UpdateRepeater(JToken data, JObject updateWith)
        {
            JArray repeaterData = data as JArray;
            foreach (var row in repeaterData)
            {
                foreach (JProperty prop in updateWith.Children())
                {
                    if (row[prop.Name] != null)
                    {
                        row[prop.Name] = prop.Value;
                    }
                }
            }

            return (JToken)repeaterData;
        }

        public static void CopyTokenValue(JToken target, Parser.Result source, string targetPath, string sourcePath)
        {
            if (source.Token != null)
            {
                JToken sourceToken = source.Token.SelectToken(sourcePath);
                if (sourceToken != null)
                {
                    JToken targetToken = target.SelectToken(targetPath);
                    if (targetToken != null)
                    {
                        var prop = targetToken.Parent as JProperty;
                        prop.Value = sourceToken;
                    }
                }
            }
            else
            {
                JToken targetToken = target.SelectToken(targetPath);
                if (targetToken != null)
                {
                    var prop = targetToken.Parent as JProperty;
                    // if source value is string and has multiselect format
                    if (source.String != null && source.String.StartsWith("[") && source.String.EndsWith("]"))
                    {
                        JArray array = JArray.Parse(source.String);
                        prop.Value = array;
                    }
                    else
                    {
                        prop.Value = double.IsNaN(source.Value) ? source.String != null ? source.String : "" : source.Value.ToString();
                    }
                }
            }
        }
    }
}
