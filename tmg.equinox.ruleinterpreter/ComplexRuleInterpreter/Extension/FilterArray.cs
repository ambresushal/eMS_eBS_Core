﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Core;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Extension
{
    class FilterArray : ParserFunction
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
            Parser.Result index = Utils.GetItem(data, ref from);
            int indexValue = Convert.ToInt32(FunctionHelper.GetArgument(index));

            result.String = currentValue.Token.ToArray()[indexValue].ToString();
            
           // ParserFunction.AddFunction(varName, new GetVarFunction(result));

            return result;
        }
    }
}
