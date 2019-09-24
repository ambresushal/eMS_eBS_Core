using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.globalUtility;
using tmg.equinox.ruleinterpreter.model;

namespace tmg.equinox.ruleinterpreter.rulecompiler
{
    public class ExpressionCompiler
    {
        public int expressionIndex = -1;
        public Regex pattern = new Regex("[()]");
        public string hasDerivedString = "hasderived";
        public char expressionOpener = '(';
        public char expressionCloser = ')';
        public List<string> expressionList = new List<string>();
        public List<string> arithmeticOperator = new List<string>() { "=", "!=", "<=", ">=", "<", ">" };


        public Dictionary<string, RuleFilterExpression> BuildRuleFilterExpressionFromString(string filterExpression)
        {
            Dictionary<string, RuleFilterExpression> ruleExpressionsDictionary = new Dictionary<string, RuleFilterExpression>();
            string[] expressions = GetExpressionFromString(filterExpression);

            for (int i = 0; i < expressions.Length; i++)
            {
                RuleFilterExpression ruleFilterExpression = new RuleFilterExpression();
                string expressionString = GetOperatorBasedExpressionString(expressions[i]);

                var operatorType = Enum.GetNames(typeof(RuleOperatorType)).Where(s => expressionString.Contains(s)).FirstOrDefault();

                if (!string.IsNullOrEmpty(operatorType))
                {
                    ruleFilterExpression.Operator = (RuleOperatorType)System.Enum.Parse(typeof(RuleOperatorType), operatorType);
                    ruleFilterExpression.ExecutionType = RuleEngineGlobalUtility.GetExecutionType(ruleFilterExpression.Operator);
                    GetRuleOperands(expressionString, ref ruleFilterExpression);
                }

                ruleFilterExpression.SequenceNumber = i + 1;
                ruleExpressionsDictionary.Add(i.ToString(), ruleFilterExpression);
            }
            return ruleExpressionsDictionary;
        }

        public string[] GetExpressionFromString(string expressionString)
        {
            int start;
            int end = expressionString.IndexOf(expressionCloser);
            if (end > -1)
            {
                start = expressionString.LastIndexOf(expressionOpener, end - 1);
                if (start == -1)
                {
                    throw new ArgumentException("Mismatched parentheses", expressionString);
                }
                expressionList.Add(pattern.Replace(expressionString.Substring(start, end - start + 1), ""));
                expressionIndex++;
                expressionString = expressionString.Substring(0, start)
                    + expressionIndex.ToString()
                    + hasDerivedString
                    + expressionString.Substring(end + 1);

                GetExpressionFromString(expressionString);
            }
            else
            {
                expressionList.Add(pattern.Replace(expressionString, ""));
            }
            return expressionList.ToArray();
        }

        public void GetRuleOperands(string expression, ref RuleFilterExpression ruleFilterObject)
        {
            string[] operandArray = expression.Split(Enum.GetNames(typeof(RuleOperatorType)), StringSplitOptions.RemoveEmptyEntries);
            string leftOperand = operandArray[0];
            string rightOperand = operandArray[1];
            ruleFilterObject.LeftOperand = ComputeRuleOperand(leftOperand);
            ruleFilterObject.RightOperand = ComputeRuleOperand(rightOperand);
        }

        public RuleOperand ComputeRuleOperand(string ruleOperandText)
        {
            RuleOperand ruleOperandObject = new RuleOperand();
            ruleOperandObject.OperandType = GetOperandType(ruleOperandText);
            ruleOperandObject.OperandPath = GetOperandPath(ruleOperandText, ruleOperandObject.OperandType);
            ruleOperandObject.OperandValue = GetOperandValue(ruleOperandText, ruleOperandObject.OperandType);

            return ruleOperandObject;
        }

        private string GetOperatorBasedExpressionString(string expressionString)
        {
            string operatorType = string.Empty;

            switch (arithmeticOperator.FirstOrDefault(expressionString.Contains))
            {

                case "=":
                    operatorType = RuleOperatorType.equalto.ToString();
                    break;

                case "!=":
                    operatorType = RuleOperatorType.notequalto.ToString();
                    break;

                case "<":
                    operatorType = RuleOperatorType.lessthan.ToString();
                    break;

                case "<=":
                    operatorType = RuleOperatorType.lessthanequalto.ToString();
                    break;

                case ">":
                    operatorType = RuleOperatorType.greaterthan.ToString();
                    break;

                case ">=":
                    operatorType = RuleOperatorType.greaterthan.ToString();
                    break;

                default:
                    operatorType = "";
                    break;

            }

            string operatorValue = arithmeticOperator.FirstOrDefault(expressionString.Contains);
            expressionString = string.IsNullOrEmpty(operatorType) ? expressionString : expressionString.Replace(operatorValue, operatorType);

            return expressionString;
        }

        public OperandType GetOperandType(string operandString)
        {
            string[] operandIdentifier = new string[] { "hasderived", "[", "." };
            OperandType operandType = OperandType.self;

            switch (operandIdentifier.FirstOrDefault(operandString.Contains))
            {
                case "hasderived":
                    operandType = OperandType.derived;
                    break;

                case "[":
                    operandType = OperandType.child;
                    break;

                case ".":
                    operandType = OperandType.property;
                    break;

                default:
                    operandType = OperandType.self;
                    break;
            }
            return operandType;
        }

        public string GetOperandPath(string operandString, OperandType operandType)
        {
            string operandPath = string.Empty;

            switch (operandType)
            {
                case OperandType.parent:
                    break;
                case OperandType.source:
                    break;
                case OperandType.target:
                    break;
                case OperandType.child:
                    operandPath = GetChildOperandPath(operandString);
                    break;
                case OperandType.self:
                    operandPath = string.Empty;
                    break;
                case OperandType.derived:
                    operandPath = GetDerivedOperandPath(operandString);
                    break;
                case OperandType.property:
                    operandPath = GetPropertyOperandPath(operandString);
                    break;
                default:
                    break;
            }
            return operandPath;
        }

        public string GetOperandValue(string operandString, OperandType operandType)
        {
            string operandValue = string.Empty;
            operandValue = operandType == OperandType.derived ? GetDerivedOperandValue(operandString) :
                                      (operandString.Contains("@") ? GetSourceBasedOperandValue(operandString)
                                      : operandString);

            return operandValue.Trim();
        }

        private string GetChildOperandPath(string operandString)
        {
            int startIndex;
            int endIndex;
            startIndex = operandString.IndexOf("[") + 1;
            endIndex = operandString.IndexOf("]", startIndex);
            return operandString.Substring(startIndex, endIndex - startIndex).Trim();
        }

        private string GetPropertyOperandPath(string operandString)
        {
            int startIndex;
            int endIndex;
            startIndex = operandString.IndexOf('.');
            endIndex = operandString.Length;
            return operandString.Substring(startIndex + 1, endIndex - (startIndex + 1)).Trim();
        }

        public string GetDerivedOperandPath(string operandString)
        {
            int startIndex;
            int endIndex;
            startIndex = operandString.IndexOf(hasDerivedString);
            endIndex = operandString.Length;
            string derivedOperandString = operandString.Replace(hasDerivedString, "");
            OperandType derivedOperandType = GetOperandType(derivedOperandString);

            return GetOperandPath(derivedOperandString, derivedOperandType).Trim();
        }

        private string GetDerivedOperandValue(string ruleOperandText)
        {
            return ruleOperandText.Substring(0, ruleOperandText.IndexOf(hasDerivedString)).Trim();
        }

        private string GetSourceBasedOperandValue(string operandString)
        {
            operandString = operandString.Contains("child") ? "child" : operandString.Substring(operandString.IndexOf('@') + 1, 1).Trim().TrimStart().TrimEnd();
            return operandString;
        }

    }
}
