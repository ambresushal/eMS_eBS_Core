using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.UIElement;

namespace tmg.equinox.web.RuleEngine.RuleDescription
{
    public abstract class RuleTextGenerator
    {
        public abstract string IterateRule(RuleRowModel rule, Dictionary<string, string> fieldList);

        public string IterateRootNode(ExpressionRowModel expression, Dictionary<string, string> elements)
        {
            List<string> exps = new List<string>();
            string logicalOpType = "";
            if (expression != null)
            {
                if (expression.Expressions != null && expression.Expressions.Count > 0)
                {
                    for (var idx = 0; idx < expression.Expressions.Count; idx++)
                    {
                        var result = expression.Expressions[idx].ExpressionTypeId == 1 ? this.IterateRootNode(expression.Expressions[idx], elements) : this.IterateLeafNode(expression.Expressions[idx], elements);
                        exps.Add(result);
                    }
                }
           
            logicalOpType = " " + RuleTextHelper.GetLogicalOperatorById(expression.LogicalOperatorTypeId) + " ";
            if (exps.Count > 1)
            {
                exps[0] = "(" + exps[0];
                exps[exps.Count - 1] = exps[exps.Count - 1] + ")";
            }
            }
            return string.Join(logicalOpType, exps);
        }

        private string IterateLeafNode(ExpressionRowModel expression, Dictionary<string, string> elements)
        {
            string expTemplate = "{0} {1} {2}";
            string leftOp = elements.Where(s => s.Key == expression.LeftOperand).Select(s => s.Value).FirstOrDefault();
            string rightOp = expression.IsRightOperandElement ? elements.Where(s => s.Key == expression.RightOperand).Select(s => s.Value).FirstOrDefault() : expression.RightOperand;
            string op = RuleTextHelper.GetOperatorTypeById(expression.OperatorTypeId);
            if ((op == "equals" || op == "not equals") && rightOp == string.Empty) rightOp = "[Blank]";
            if (expression.LeftKeyFilter != null && expression.LeftKeyFilter.Count > 0)
            {
                var leftKeyDesc = GetKeyFilterDescription(expression.LeftKeyFilter);
                if (!String.IsNullOrEmpty(leftKeyDesc))
                {
                    leftOp = leftOp + leftKeyDesc;
                }
            }
            if (expression.RightKeyFilter != null && expression.RightKeyFilter.Count > 0)
            {
                var rightKeyDesc = GetKeyFilterDescription(expression.RightKeyFilter);
                if (!String.IsNullOrEmpty(rightKeyDesc))
                {
                    rightOp = rightOp + rightKeyDesc;
                }
            }
            if (expression.CompOp != null)
            {
                var factor = expression.CompOp.Factor;
                if (!string.IsNullOrWhiteSpace(factor))
                {
                    var factorVal = expression.CompOp.FactorValue;
                    var factorDesc = factorVal + GetFactorDescripttion(expression.CompOp.Factor);
                    op = op + " " + factorDesc;
                }
            }

            return string.Format(expTemplate, leftOp, op, rightOp);
        }
        private string GetKeyFilterDescription(List<RepeaterKeyFilterModel> keyFilter)
        {
            string desc = string.Empty;
            try
            {
                if (keyFilter != null && keyFilter.Count > 0)
                {
                    int keyCnt = keyFilter.Count;
                    desc = "[";
                    for (int i = 0; i < keyCnt; i++)
                    {
                        desc += keyFilter[i].Label + "=" + keyFilter[i].FilterValue + (i != keyCnt - 1 ? " and " : String.Empty);
                    }
                    desc += "]";
                }
            }
            catch (Exception)
            {
            }
            return desc;
        }
        private string GetFactorDescripttion(string factor)
        {
            string factorDesc = String.Empty;
            try
            {
                if (!String.IsNullOrEmpty(factor))
                {
                    switch (factor)
                    {
                        case "+":
                            factorDesc = " more of";
                            break;
                        case "*":
                            factorDesc = " times";
                            break;
                        case "%":
                            factorDesc = "% of";
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }
            return factorDesc;
        }
    }
}
