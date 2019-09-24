using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Core;

namespace tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Extension
{
    class RowColumnValue : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result result = new Parser.Result();

            // 1. Get the name of the variable.
            string varName = Utils.GetToken(data, ref from, Constants.NEXT_ARG_ARRAY);
            if (from >= data.Length)
            {
                throw new ArgumentException("Couldn't get variable");
            }

            // 2. Get the current value of the variable.
            ParserFunction func = ParserFunction.GetFunction(varName);
            Parser.Result currentValue = func.GetValue(data, ref from);
            
            Parser.Result columnName = Utils.GetItem(data, ref from);
            
            result.Token = GetColumnValues(currentValue.Token, columnName.String);
           
            ParserFunction.AddFunction(varName, new GetVarFunction(result));

            return result;
        }

        private JToken GetColumnValues(JToken data, string columnName)
        {
            JToken result = null;
            List<JToken> t = (from r in data select r).ToList();

            if (t.Count > 0)
            {
                result = t[0][columnName];
                
                if (t.Count > 1 && result !=null )
                {
                   var columnData= t.Select(sel=>sel[columnName].ToString()).ToList();
                   result = JToken.FromObject(string.Join(",", columnData.Distinct()));

                }
            }

            return result;
        }
    }
}
