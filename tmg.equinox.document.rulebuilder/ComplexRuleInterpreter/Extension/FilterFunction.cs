using tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Helper;

namespace tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Extension
{
    class FilterFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result result = new Parser.Result();

            // 1. Get the variable name
            string varName = Utils.GetToken(data, ref from, Constants.NEXT_ARG_ARRAY);
            if (from >= data.Length)
            {
                throw new ArgumentException("Couldn't set variable before end of line");
            }

            // 2. Get the current value of the variable.
            ParserFunction func = ParserFunction.GetFunction(varName);
            Parser.Result currentValue = func.GetValue(data, ref from);

            // 3. Get the Filter Condition parameter value
            Parser.Result filterCondition = Utils.GetItem(data, ref from);

            // 4. Get the column list seperated by comma
            Parser.Result columns = Utils.GetItem(data, ref from);

            // 5. Filter the source by columns and return the result
            if (currentValue.Token.Type != JTokenType.Array)
            {
                throw new ArgumentException("invalid source type.");
            }

            Condition cond = JsonConvert.DeserializeObject<Condition>(filterCondition.String);


            result.Token = string.Equals(cond.LogicalOperator, "AND", StringComparison.OrdinalIgnoreCase) ?
                                                FunctionHelper.EvaluteAnd(currentValue.Token, cond, columns.String) :
                                                FunctionHelper.EvaluteOr(currentValue.Token, cond, columns.String);

            return result;
        }

        
    }
    
}
