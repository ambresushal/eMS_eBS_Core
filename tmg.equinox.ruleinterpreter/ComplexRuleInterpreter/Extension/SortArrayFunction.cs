using Newtonsoft.Json.Linq;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Helper;
using tmg.equinox.ruleinterpreter.jsonhelper;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Extension
{
    class SortArrayFunction : ParserFunction
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

            // 2. Get the current value of the variable.
            ParserFunction func = ParserFunction.GetFunction(varName);
            Parser.Result currentValue = func.GetValue(data, ref from);

            // 3. Get the value of second parameter
            Parser.Result newValue = Utils.GetItem(data, ref from);

            // 4. Get the value to be replaced
            Parser.Result repValue = Utils.GetItem(data, ref from);

            string arg1 = FunctionHelper.GetArgument(currentValue);
            string arg2 = FunctionHelper.GetArgument(newValue);
            string arg3 = FunctionHelper.GetArgument(repValue);
            JToken SortToken = null;
            try
            {
                if (currentValue.Token != null)
                {
                    SortToken = currentValue.Token;
                }
                else
                {
                    if (currentValue.String.StartsWith("[") && currentValue.String.EndsWith("]"))
                    {
                        SortToken = JToken.Parse(currentValue.String);
                    }
                }
                if (SortToken != null)
                {
                    List<JToken> SortedList = ApplySort(SortToken, arg2, arg3);
                    result.Token = JSONHelper.ConvertJtokenListToJToken(SortedList);
                }
                else
                {
                    result = currentValue;
                }

            }
            catch (Exception)
            {
                throw new ArgumentException("Invalid token for Sorting...");
            }
            return result;
        }

        private List<JToken> ApplySort(JToken token, string sortingColumn, string sortDiraction)
        {
            List<JToken> TokenList = (from r in token select r).ToList();
            if (TokenList.Count() > 0)
            {
                string[] colum = new string[] { "" };
                if (!string.IsNullOrEmpty(sortingColumn))
                {
                    colum = sortingColumn.Split(',');
                }
                foreach (var item in colum)
                {
                    if (!string.IsNullOrEmpty(item))
                        TokenList = Sort(TokenList, item, sortDiraction);
                }
            }
            return TokenList;
        }

        private List<JToken> Sort(List<JToken> TokenList, string sortingColumn, string sortDiraction)
        {
                sortDiraction = string.IsNullOrEmpty(sortDiraction) == true ? "asc" : sortDiraction;
                if (sortDiraction.Equals("asc"))
                {
                    TokenList = TokenList
                        .OrderBy(s => s[sortingColumn])
                        .ToList();
                }
                else if (sortDiraction.Equals("desc"))
                {
                    TokenList = TokenList
                    .OrderByDescending(s => s[sortingColumn])
                    .ToList();
                }
            return TokenList;
        }
    }
}
