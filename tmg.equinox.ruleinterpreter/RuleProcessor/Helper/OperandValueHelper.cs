using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.ruleinterpreter.jsonhelper;

namespace tmg.equinox.ruleinterpreter.ruleProcessor.operandhelper
{
    public static class OperandValueHelper
    {

        public static List<JToken> GetDerivedSourceValue(this RuleOperand ruleOperand, Dictionary<string, JToken> expressionsOutput)
        {
            List<JToken> sourceValues = new List<JToken>();
            JToken expressionValue = expressionsOutput[ruleOperand.OperandValue];

            if (!expressionValue.IsNullOrEmpty())
            {
                if (!string.IsNullOrEmpty(ruleOperand.OperandPath))
                {
                    expressionValue = expressionValue.FirstOrDefault().SelectToken(ruleOperand.OperandPath);
                    sourceValues = expressionValue.ToList();
                }
                else
                {
                    sourceValues = expressionValue.ToList();
                }
            }
            return sourceValues;
        }

        public static List<JToken> GetChildOperandValue(this RuleOperand ruleOperand, Dictionary<string, JToken> sources)
        {
            List<JToken> operandOutput = new List<JToken>();
            JToken sourceItem = sources[ruleOperand.OperandValue];

            if (!sourceItem.IsNullOrEmpty())
            {
                if (!string.IsNullOrEmpty(ruleOperand.OperandPath))
                {
                    operandOutput = sourceItem.ToList().Select(sel => sel[ruleOperand.OperandPath]).ToList();
                }
                else
                {
                    operandOutput = sourceItem.ToList();
                }
            }

            return operandOutput;
        }

        public static string GetPropertyOperandValue(this RuleOperand ruleOperand, Dictionary<string, JToken> sources)
        {
            string propertyValue = string.Empty;

            if (!string.IsNullOrEmpty(ruleOperand.OperandPath))
            {
                propertyValue = sources[ruleOperand.OperandValue].FirstOrDefault().SelectToken(ruleOperand.OperandPath).ToString();
            }
            else
            {
                propertyValue = ruleOperand.OperandValue.Trim().ToString();
            }

            return propertyValue;
        }

        public static List<JToken> GetSelfOperandValue(this RuleOperand ruleOperand, Dictionary<string, JToken> sources)
        {
            List<JToken> selfOperandValues = new List<JToken>();
            JToken selfOpernadToken = sources[ruleOperand.OperandValue];

            if (!selfOpernadToken.IsNullOrEmpty())
                selfOperandValues = sources[ruleOperand.OperandValue].ToList();

            return selfOperandValues;
        }

    }
}
