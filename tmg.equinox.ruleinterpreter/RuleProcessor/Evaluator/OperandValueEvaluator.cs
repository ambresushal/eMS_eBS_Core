using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.ruleinterpreter.ruleProcessor.operandhelper;

namespace tmg.equinox.ruleinterpreter.evaluator
{
    public class OperandValueEvaluator
    {
        Dictionary<string, JToken> _sources;
        Dictionary<string, JToken> _expressionsOutput;

        public OperandValueEvaluator(Dictionary<string, JToken> sources, Dictionary<string, JToken> expressionsOutput)
        {
            _sources = sources;
            _expressionsOutput = expressionsOutput;
        }

        public List<JToken> GetOperandValue(RuleOperand ruleOperand)
        {
            List<JToken> operandValue = new List<JToken>();

            switch (ruleOperand.OperandType)
            {
                case OperandType.parent:
                    break;
                case OperandType.source:
                    break;
                case OperandType.target:
                    break;
                case OperandType.child:
                case OperandType.property:
                    operandValue =ruleOperand.GetChildOperandValue(_sources); //   Get the values for operand with @A[Service.ServiceList] or @A.BenefitCategory
                    break;
                case OperandType.self:
                    operandValue =ruleOperand.GetSelfOperandValue(_sources);  // Get values for Self result with No ChildPath
                    break;
                case OperandType.derived:
                    operandValue = ruleOperand.GetDerivedSourceValue(_expressionsOutput);// Get value operand with Derived expression 
                    break;
                default:
                    break;
            }
            return operandValue;
        }
    }
}
