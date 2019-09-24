using Newtonsoft.Json.Linq;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Extension
{
    class SetSourceFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            string varName = Utils.GetToken(data, ref from, Constants.NEXT_ARG_ARRAY);
            if (from >= data.Length)
            {
                throw new ArgumentException("Couldn't set variable before end of line");
            }

            Parser.Result varValue = Utils.GetItem(data, ref from);

            JToken val = SourceManager.Get(Thread.CurrentThread, varValue.String);
            if (val == null)
            {
                throw new ArgumentException("Couldn't find source.");
            }
            varValue.Token = val;
            varValue.String = null;

            //if (val is JArray)
            //{
            //    varValue.Token = val;
            //    varValue.String = null;

            //}
            //else
            //{
            //    varValue.Token = null;
            //    varValue.String = val.ToString();
            //}
            ParserFunction.AddFunction(varName, new GetVarFunction(varValue));

            return new Parser.Result(Double.NaN, varName);
        }
    }
}
