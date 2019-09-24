using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.viewmodels.UIElement;

namespace tmg.equinox.web.RuleEngine.RuleDescription
{
    public class RuleTextManager
    {
        List<RuleRowModel> _ruleList;
        public RuleTextManager()
        {
        }
        public RuleTextManager(List<RuleRowModel> rules)
        {
            this._ruleList = rules;
        }

        public List<RuleRowModel> GenerateRuleText(List<UIElementRowModel> elements)
        {
            Dictionary<string, string> fieldList = this.GetElementLabel(elements);

            foreach (var item in _ruleList)
            {
                var objGenerator = RuleTextGeneratorFactory.GetGenerator(item.TargetPropertyId);
                string description = objGenerator.IterateRule(item, fieldList);
                item.RuleDescription = description;
            }
            return _ruleList;
        }

        public RuleRowModel GenerateRuleText(RuleRowModel rule, List<UIElementRowModel> elements)
        {
            Dictionary<string, string> fieldList = this.GetElementLabel(elements);
            var objGenerator = RuleTextGeneratorFactory.GetGenerator(rule.TargetPropertyId);
            string description = objGenerator.IterateRule(rule, fieldList);
            rule.RuleDescription = description;
            return rule;
        }

        private Dictionary<string, string> GetElementLabel(List<UIElementRowModel> elements)
        {
            Dictionary<string, string> fieldNames = new Dictionary<string, string>();

            foreach (var element in elements)
            {
                if (!fieldNames.ContainsKey(element.UIElementName))
                    fieldNames.Add(element.UIElementName, element.Label);
            }

            return fieldNames;
        }

        public List<string> GetLeftOperands()
        {
            List<string> elements = new List<string>();

            foreach (var rule in _ruleList)
            {
                if (rule.RootExpression != null)
                {
                    if (rule.RootExpression.Expressions != null)
                    {
                        foreach (var expression in rule.RootExpression.Expressions)
                        {
                            GetOperands(expression, ref elements);
                        }
                    }
                }
            }

            return elements;
        }

        public List<string> GetLeftOperands(RuleRowModel rule)
        {
            List<string> elements = new List<string>();
            {
                if (rule.RootExpression != null)
                {
                    if (rule.RootExpression.Expressions != null)
                    {
                        foreach (var expression in rule.RootExpression.Expressions)
                        {
                            GetOperands(expression, ref elements);
                        }
                    }
                }
            }

            return elements;
        }

        private void GetOperands(ExpressionRowModel expression, ref List<string> elements)
        {
            string operandName = string.Empty;
            if (expression.ExpressionTypeId == 1)
            {
                if (expression.Expressions != null)
                {
                    foreach (var exp in expression.Expressions)
                    {
                        GetOperands(exp, ref elements);
                    }
                }
            }
            else
            {
                elements.Add(expression.LeftOperand);
                if (expression.IsRightOperandElement)
                {
                    elements.Add(expression.RightOperand);
                }
            }
        }

    }
}
