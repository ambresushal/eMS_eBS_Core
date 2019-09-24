using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Core;
using tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Helper;
using tmg.equinox.document.rulebuilder.jsonhelper;


namespace tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Extension
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
                }
            }

            return count;
        }

        public static JToken EvaluteIndex(JToken data, int index, string colList)
        {
            JToken output = null;
            if (index < ((JArray)data).Count)
            {
                JToken t = ((JArray)data)[index];

                if (!colList.Contains(',') && !t.IsNullOrEmpty())
                {
                    output = t.SelectToken(colList);
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
            var result = new object();

            foreach (var exp in rule.Expression)
            {
                t = (from r in t where (string)r[exp.Column] == exp.Value select r).ToList();
            }

            if (!colList.Contains(',') && t.Count == 1)
            {
                result = JToken.FromObject(t[0][colList]);
            }
            else
            {
                result = GetDataByColumn(t, colList);
            }

            return JToken.FromObject(result);
        }

        private static JArray GetDataByColumn(List<JToken> data, string columns)
        {
            JArray repeaterRows = new JArray();

            string[] cols = columns.Split(',');
            if (cols.Length > 1)
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
                expr.Column = expression.Substring(0, expression.IndexOf('='));
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



        private static string HandleKeyCharacter(string expressionString)
        {
            var pattern = @"(?<=\[)(.*?)(?=\])";
            var matchedStrings = Regex.Matches(expressionString, pattern).Cast<Match>().ToList();

            if (matchedStrings.FindAll(x => x.Value.Contains('&') || x.Value.Contains('|')).Count > 0)
            {
                foreach (var matchedItem in matchedStrings)
                {
                    expressionString = expressionString.Replace(matchedItem.ToString(), matchedItem.ToString().Replace("&", "\\and").Replace("|", "\\or"));
                }
            }
            return expressionString;
        } 

    }
}
