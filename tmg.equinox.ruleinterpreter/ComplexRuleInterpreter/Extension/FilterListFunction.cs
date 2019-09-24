using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Core;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Helper;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Extension
{
    class FilterListFunction : ParserFunction
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
            Parser.Result sourceValue = func.GetValue(data, ref from);

            // 3. Get the value of Filter
            Parser.Result rowFilter = Utils.GetItem(data, ref from);

            // 4. Get the value of Child Container
            Parser.Result childContainer = Utils.GetItem(data, ref from);

            // 4. Get the value of Child Filter
            Parser.Result childFilter = Utils.GetItem(data, ref from);

            // 5. Get the value of column list
            Parser.Result columnValues = Utils.GetItem(data, ref from);

            string arg1 = FunctionHelper.GetArgument(rowFilter);
            string arg2 = FunctionHelper.GetArgument(childFilter);
            string arg3 = FunctionHelper.GetArgument(columnValues);
            string arg4 = FunctionHelper.GetArgument(childContainer);

            JToken token = null;
            if (!double.IsNaN(rowFilter.Value))
            {
                token = FunctionHelper.EvaluteIndex(sourceValue.Token, Convert.ToInt32(rowFilter.Value), !string.IsNullOrEmpty(arg2) ? arg4 : arg3, arg4);
            }
            else if (!string.IsNullOrEmpty(arg1))
            {
                Condition rowFilterCondition = FunctionHelper.GetCondition(arg1);
                token = string.Equals(rowFilterCondition.LogicalOperator, "AND", StringComparison.OrdinalIgnoreCase) ?
                    FunctionHelper.EvaluteAnd(sourceValue.Token, rowFilterCondition, !string.IsNullOrEmpty(arg4) ? arg4 : arg3) :
                                                FunctionHelper.EvaluteOr(sourceValue.Token, rowFilterCondition, !string.IsNullOrEmpty(arg4) ? arg4 : arg3);
            }
            else
            {
                JArray filteredColumn = FunctionHelper.GetDataByColumn(sourceValue.Token.ToList(), arg3);
                token = JToken.FromObject(filteredColumn);
            }

            if (!double.IsNaN(childFilter.Value))
            {
                token = FunctionHelper.EvaluteIndex(token, Convert.ToInt32(childFilter.Value), arg3, "");
            }
            else if (!string.IsNullOrEmpty(arg2))
            {
                Condition rowFilterCondition = FunctionHelper.GetCondition(arg2);
                token = string.Equals(rowFilterCondition.LogicalOperator, "AND", StringComparison.OrdinalIgnoreCase) ?
                                                FunctionHelper.EvaluteAnd(token, rowFilterCondition, arg3) :
                                                FunctionHelper.EvaluteOr(token, rowFilterCondition, arg3);
            }
            else if (!string.IsNullOrEmpty(arg4))
            {
                JArray filteredColumn = FunctionHelper.GetDataByColumn(token.ToList(), arg3);
                token = JToken.FromObject(filteredColumn);
            }

            result.Token = token;
            return result;
        }
    }
}
