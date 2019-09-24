using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.UIElement;

namespace tmg.equinox.web.RuleEngine.RuleDescription
{
    public class EnableRuleTextGenerator : RuleTextGenerator
    {
        private string _descTemplate = "Enable this field if {0}";
        public override string IterateRule(RuleRowModel rule, Dictionary<string, string> fieldList)
        {
            string result = this.IterateRootNode(rule.RootExpression, fieldList);
            if (result.StartsWith("(") && result.EndsWith(")"))
                result = string.Join("", result.Skip(1).Take(result.Length - 2));
            return string.Format(_descTemplate, result);
        }
    }

    public class VisibleRuleTextGenerator : RuleTextGenerator
    {
        private string _descTemplate = "Display this field if {0}";
        public override string IterateRule(RuleRowModel rule, Dictionary<string, string> fieldList)
        {
            string result = this.IterateRootNode(rule.RootExpression, fieldList);
            if (result.StartsWith("(") && result.EndsWith(")"))
                result = string.Join("", result.Skip(1).Take(result.Length - 2));
            return string.Format(_descTemplate, result);
        }
    }

    public class ValueRuleTextGenerator : RuleTextGenerator
    {
        private string _descTemplate = "If {0} then set {1} else {2}";
        public override string IterateRule(RuleRowModel rule, Dictionary<string, string> fieldList)
        {
            string result = this.IterateRootNode(rule.RootExpression, fieldList);
            if (result.StartsWith("(") && result.EndsWith(")"))
                result = string.Join("", result.Skip(1).Take(result.Length - 2));
            return string.Format(_descTemplate, result, rule.ResultSuccess, rule.ResultFailure);
        }
    }

    public class CustomRuleTextGenerator : RuleTextGenerator
    {
        private string _descTemplate = "Execute the following fuction {0} if {0}";
        public override string IterateRule(RuleRowModel rule, Dictionary<string, string> fieldList)
        {
            string result = this.IterateRootNode(rule.RootExpression, fieldList);
            if (result.StartsWith("(") && result.EndsWith(")"))
                result = string.Join("", result.Skip(1).Take(result.Length - 2));
            return string.Format(_descTemplate, rule.ResultSuccess, result);
        }
    }

    public class DialogRuleTextGenerator : RuleTextGenerator
    {
        private string _descTemplate = "Display confirmation dialog if {0}";
        public override string IterateRule(RuleRowModel rule, Dictionary<string, string> fieldList)
        {
            string result = this.IterateRootNode(rule.RootExpression, fieldList);
            if (result.StartsWith("(") && result.EndsWith(")"))
                result = string.Join("", result.Skip(1).Take(result.Length - 2));
            return string.Format(_descTemplate, result);
        }
    }

    public class ErrorRuleTextGenerator : RuleTextGenerator
    {
        private string _descTemplate = "Display error if not {0}";
        public override string IterateRule(RuleRowModel rule, Dictionary<string, string> fieldList)
        {
            string result = this.IterateRootNode(rule.RootExpression, fieldList);
            if (result.StartsWith("(") && result.EndsWith(")"))
                result = string.Join("", result.Skip(1).Take(result.Length - 2));
            return string.Format(_descTemplate, result);
        }
    }

    public class HighlightRuleTextGenerator : RuleTextGenerator
    {
        private string _descTemplate = "Highlight this field if {0}";
        public override string IterateRule(RuleRowModel rule, Dictionary<string, string> fieldList)
        {
            string result = this.IterateRootNode(rule.RootExpression, fieldList);
            if (result.StartsWith("(") && result.EndsWith(")"))
                result = string.Join("", result.Skip(1).Take(result.Length - 2));
            return string.Format(_descTemplate, result);
        }
    }

    public class RequiredRuleTextGenerator : RuleTextGenerator
    {
        private string _descTemplate = "This field is required if {0}";
        public override string IterateRule(RuleRowModel rule, Dictionary<string, string> fieldList)
        {
            string result = this.IterateRootNode(rule.RootExpression, fieldList);
            if (result.StartsWith("(") && result.EndsWith(")"))
                result = string.Join("", result.Skip(1).Take(result.Length - 2));
            return string.Format(_descTemplate, result);
        }
    }

    public class ValidationRuleTextGenerator : RuleTextGenerator
    {
        private string _descTemplate = "Validate this field if {0}";
        public override string IterateRule(RuleRowModel rule, Dictionary<string, string> fieldList)
        {
            string result = this.IterateRootNode(rule.RootExpression, fieldList);
            if (result.StartsWith("(") && result.EndsWith(")"))
                result = string.Join("", result.Skip(1).Take(result.Length - 2));
            return string.Format(_descTemplate, result);
        }
    }
}
