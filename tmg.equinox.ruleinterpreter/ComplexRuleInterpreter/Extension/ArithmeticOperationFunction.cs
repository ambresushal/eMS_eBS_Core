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
    class ArithmeticOperationFunction : ParserFunction
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

            // 4. Take either the string part if it is defined,
            // or the numerical part converted to a string otherwise.

            string arg1 = FunctionHelper.GetArgument(currentValue);
            string arg2 = FunctionHelper.GetArgument(newValue);
            string arg3 = FunctionHelper.GetArgument(repValue);
            
            // 5. The variable becomes a string after adding a string to it.
            if (!string.IsNullOrEmpty(arg1) & !string.IsNullOrEmpty(arg2) & !string.IsNullOrEmpty(arg3))
            {
                double Val1, Val2;
                bool isDoubleArg1, isDoubleArg3;

                isDoubleArg1 = double.TryParse(arg1, out Val1);
                if(isDoubleArg1 == false)
                     arg1 = "0";

                isDoubleArg3 = double.TryParse(arg3, out Val2);
                if (isDoubleArg3 == false)
                    arg3 = "0";

                Val1 = double.Parse(arg1);
                Val2 = double.Parse(arg3);

                switch (arg2)
                {
                    case "-":
                        result.String = Subtraction(Val1, Val2).ToString();
                        break;
                    case "/":
                        result.String = Division(Val1, Val2).ToString();
                        break;
                    case "*":
                        result.String = Multiplication(Val1, Val2).ToString();
                        break;
                    case "+":
                        result.String = Addition(Val1, Val2).ToString();
                        break;
                }
            }
            if (result.String == "NaN")
                result.String = "";
            return result;
        }

        private double Subtraction(double arg1, double arg2)
        {
            return arg1 - arg2;
        }

        private double Division(double arg1, double arg2)
        {
            double result = 0;
            try
            {
                result = (arg1 / arg2);
            }
            catch (DivideByZeroException e)
            {

            }
            return result;
        }

        private double Multiplication(double arg1, double arg2)
        {
            double result = 0;
            try
            {
                result = (arg1 * arg2);
            }
            catch (DivideByZeroException e)
            {

            }
            return result;
        }

        private double Addition(double arg1, double arg2)
        {
            return arg1 + arg2;
        }
    }

    class MathRoundOff : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            // 1. Get the name of the variable.
            string varName = Utils.GetToken(data, ref from, Constants.NEXT_ARG_ARRAY);
            if (from >= data.Length)
            {
                throw new ArgumentException("Couldn't get variable");
            }

            // 2. Get the current value of the variable.
            ParserFunction func = ParserFunction.GetFunction(varName);
            Parser.Result currentValue = func.GetValue(data, ref from);

            // 3. Get the value of second parameter
            Parser.Result result = Utils.GetItem(data, ref from);
            string arg2 = FunctionHelper.GetArgument(result);
            string arg1 = FunctionHelper.GetArgument(currentValue);

            if (!string.IsNullOrEmpty(arg1) & !string.IsNullOrEmpty(arg2))
            {
                double Val1, Val3 = 0;
                Val1 = double.Parse(arg1);

                if (arg2.Equals("100f"))
                {
                    Val3 = Math.Round(Val1 * 100f) / 100f;
                }
                result.String = Val3.ToString();
            }
            if (result.String == "100f")
                result.String = "";
            return result;
        }
    }

    class SBCRoundOff : ParserFunction
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

            string arg1 = FunctionHelper.GetArgument(currentValue);
            string arg2 = FunctionHelper.GetArgument(newValue);

            //if (!string.IsNullOrEmpty(arg1) & !string.IsNullOrEmpty(arg2))
            //{
            result.String = RoundOff(arg1);
            //}
            return result;
        }

        private string RoundOff(string val)
        {
            string result;
            double retVal = 0;
            if (!string.IsNullOrEmpty(val))
            {
                double sourceVal = double.Parse(val);
                sourceVal = Math.Round(sourceVal, 0);
                int length = sourceVal.ToString().Length;
                if (sourceVal > 0)
                    retVal = length == 1 ? sourceVal : length == 2 ? Round(sourceVal, 10) : Round(sourceVal, 100);
            }
            result = retVal.ToString();
            return result;
        }

        private double Round(double sourceVal, int nearest)
        {
            if (sourceVal % 50 != 0 && sourceVal % 10 == 0)
            {
                sourceVal = sourceVal + 1;
            }
            string strVal = (sourceVal / nearest).ToString();
            int index = strVal.IndexOf(".");
            double result = index == -1 ? sourceVal :
                (double.Parse(strVal.Remove(0, index + 1)) > (nearest / 2) ?
                    Math.Ceiling(sourceVal / nearest) * nearest :
                    Math.Ceiling(sourceVal / nearest) * nearest - nearest);
            return result;
        }
    }
}
