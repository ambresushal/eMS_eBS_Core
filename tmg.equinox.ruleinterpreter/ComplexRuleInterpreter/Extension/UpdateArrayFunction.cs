using Newtonsoft.Json.Linq;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Helper;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Extension
{
    class UpdateArrayFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result result = new Parser.Result();

            // 1. Get the name of the variable.
            string varName = Utils.GetToken(data, ref from, Constants.NEXT_ARG_ARRAY);
            if (from >= data.Length)
            {
                throw new ArgumentException("Couldn't find variable");
            }

            // 2. Get the value of the variable
            ParserFunction func = ParserFunction.GetFunction(varName);
            Parser.Result currentValue = func.GetValue(data, ref from);

            // 3. Get the value of second parameter
            Parser.Result filterCondition = Utils.GetItem(data, ref from);
            Condition rowFilter = FunctionHelper.GetCondition(filterCondition.String);

            Condition childCondition = null;
            string childContainer = string.Empty;
            List<Expression> childExp = rowFilter.Expression.Where(s => s.Container != null).ToList();
            if (childExp.Count > 0)
            {
                rowFilter.Expression = rowFilter.Expression.Where(s => s.Container == null).ToList();
                childContainer = childExp[0].Container;
                childCondition = new Condition();
                childCondition.LogicalOperator = rowFilter.LogicalOperator;
                childCondition.Expression = childExp;
            }


            List<JToken> filterRows = GetFilteredRow(currentValue.Token, rowFilter);
            //JToken filterColmuns = filterRows;
            //if (childExp.Count > 0)
            //{
            //    filterColmuns = string.Equals(childCondition.LogicalOperator, "AND", StringComparison.OrdinalIgnoreCase) ?
            //                        FunctionHelper.EvaluteAnd(filterRows, childCondition, string.Empty) :
            //                        FunctionHelper.EvaluteOr(filterRows, childCondition, string.Empty);
            //}

            // 4. Get the value of third parameter
            Parser.Result updateValue = Utils.GetItem(data, ref from);
            try
            {
                if (filterRows != null && filterRows.Count() > 0)
                {
                  string jsonString= updateValue.Token != null ? updateValue.Token.ToString() : updateValue.String;
                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        jsonString = jsonString.Replace("\"[", "[");
                        jsonString = jsonString.Replace("]\"", "]");
                    }
                    JObject updateWith = JObject.Parse(jsonString);
                    foreach (var row in filterRows)
                    {
                        foreach (JProperty prop in updateWith.Children())
                        {
                            if (row[prop.Name] != null)
                            {
                                row[prop.Name] = prop.Value;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new ArgumentException("Invalid token for update...");
            }

            result.Token = currentValue.Token;

            return result;
        }

        private List<JToken> GetFilteredRow(JToken data, Condition rule)
        {
            List<JToken> t = (from r in data select r).ToList();

            foreach (var exp in rule.Expression)
            {
                t = (from r in t where (string)r[exp.Column] == exp.Value select r).ToList();

                if (!string.Equals(rule.LogicalOperator, "AND", StringComparison.OrdinalIgnoreCase))
                {
                    if (t.Count > 0)
                    {
                        break;
                    }
                }
            }

            return t;
        }
    }
}
