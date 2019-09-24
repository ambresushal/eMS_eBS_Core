using Newtonsoft.Json.Linq;
using tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Extension
{
    class SetTokenValueFunction : ParserFunction
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
            if (ParserFunction.FunctionExists(varName))
            {
                ParserFunction func = ParserFunction.GetFunction(varName);

                Parser.Result currentValue = func.GetValue(data, ref from);

                Parser.Result sourceValue = Utils.GetItem(data, ref from);

                Parser.Result targetPath = Utils.GetItem(data, ref from);

                Parser.Result sourcePath = Utils.GetItem(data, ref from);

                if (targetPath.String != null)
                {
                    FunctionHelper.CopyTokenValue(currentValue.Token, sourceValue,  targetPath.String, sourcePath.String);
                }
            }
            else
            {
                JToken targetVal = SourceManager.Get(Thread.CurrentThread, "target");

                Parser.Result sourceValue = Utils.GetItem(data, ref from);

                Parser.Result targetPath = Utils.GetItem(data, ref from);

                Parser.Result sourcePath = Utils.GetItem(data, ref from);

                if (targetPath.String != null)
                {
                    FunctionHelper.CopyTokenValue(targetVal, sourceValue, targetPath.String, sourcePath.String);
                }
                SourceManager.Set(Thread.CurrentThread, "target", targetVal);
            }

            return result;
        }
    }
}
