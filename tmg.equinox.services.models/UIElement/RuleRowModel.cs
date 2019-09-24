using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.UIElement
{
    public class RuleRowModel
    {
        public int TenantId { get; set; }

        public int FormDesignVersionId { get; set; }

        //UIElementID of the UIElement
        public int UIElementID { get; set; }

        public string RuleName { get; set; }

        public int PropertyRuleMapID { get; set; }

        public int RuleId { get; set; }

        public string RuleDescription { get; set; }

        public int TargetPropertyId { get; set; }

        public string TargetProperty { get; set; }
        public List<RepeaterKeyFilterModel> TargetKeyFilter { get; set; }

        public bool IsCustomRule { get; set; }

        public string ResultSuccess { get; set; }

        public string ResultFailure { get; set; }

        public IEnumerable<ExpressionRowModel> Expressions { get; set; }

        public ExpressionRowModel RootExpression { get; set; }

        public bool IsResultSuccessElement { get; set; }

        public bool IsResultFailureElement { get; set; }

        public string Message { get; set; }

        public bool RunOnLoad { get; set; }
        public bool IsStandard { get; set; }

        public int SourceRuleID { get; set; }

    }
}
