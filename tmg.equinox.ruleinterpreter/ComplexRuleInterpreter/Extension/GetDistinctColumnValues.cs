using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Core;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Extension
{
    class GetDistinctColumnValues : ParserFunction
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

            try
            {

            }
            catch (Exception)
            {

                throw;
            }
            result.Token = GetColumnValues(currentValue.Token, columnName.String);
           
            //ParserFunction.AddFunction(varName, new GetVarFunction(result));

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
                    try
                    {

                        var columnData = t.Select(sel => sel.SelectToken(columnName).ToString()).Distinct().ToList();
                        result = JToken.FromObject(string.Join(",", columnData.Distinct().Where(m => !string.IsNullOrEmpty(m))));
                    }
                    catch   ( Exception ex)
                    {
                        var mm= t.Select(sel => sel.SelectToken(columnName)).Distinct().ToList();

                    }
                }
            }

            return result;
        }
    }
}
