using Newtonsoft.Json.Linq;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Extension
{
    class ExtractSnippetFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result result = new Parser.Result();


            string varName = Utils.GetToken(data, ref from, Constants.NEXT_ARG_ARRAY);
            if (from >= data.Length)
            {
                throw new ArgumentException("Couldn't get variable");
            }

            ParserFunction func = ParserFunction.GetFunction(varName);
            Parser.Result currentValue = func.GetValue(data, ref from);
            string inputString = FunctionHelper.GetArgument(currentValue);

            Parser.Result actionInput = Utils.GetItem(data, ref from);
            string action = FunctionHelper.GetArgument(actionInput);

            Parser.Result valueToReplace = Utils.GetItem(data, ref from);
            string replaceString = FunctionHelper.GetArgument(valueToReplace);

            Parser.Result snippetValue= Utils.GetItem(data, ref from);
            string snippetString = FunctionHelper.GetArgument(snippetValue);


            string resultString = inputString;
            string getActionResult = string.Empty;
            string regexHTMLComment = @"<!--"+snippetString+"-->";
            var matches = Regex.Matches(inputString, regexHTMLComment);

            if (matches.Count == 2 && !string.IsNullOrEmpty(action))
            {
                List<Capture> captures = new List<Capture>();
                foreach (var match in matches)
                {
                    captures.Add(((Match)match).Captures[0]);
                }
                var cap = captures.OrderBy(a => a.Index).ToList();
                int startIndex = cap[0].Index + cap[0].Length;
                int endIndex = cap[1].Index;
                resultString = inputString.Substring(startIndex, endIndex - startIndex);
                getActionResult = inputString.Substring(startIndex, endIndex - startIndex);

                if (action.ToUpper() == "GET")
                {
                    resultString = getActionResult;
                }

                else if (action.ToUpper() == "REPLACE")
                {
                    int snippetLength = regexHTMLComment.Length;
                    int replaceStartIndex = startIndex - snippetLength;
                    int countToReplace = getActionResult.Length + (snippetLength * 2);

                    inputString = inputString.Remove(replaceStartIndex, countToReplace);
                    inputString = inputString.Insert(replaceStartIndex, replaceString);
                    resultString = inputString;
                }
            }
            result.String = resultString;
            return result;
        }
    }
}
