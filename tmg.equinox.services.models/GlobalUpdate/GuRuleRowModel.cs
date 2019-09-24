using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.UIElement;

namespace tmg.equinox.applicationservices.viewmodels.GlobalUpdate
{
    public class GuRuleRowModel : ViewModelBase
    {
        public int UIElementID { get; set; }
        public int RuleId { get; set; }
        public int TargetPropertyId { get; set; }
        public string TargetProperty { get; set; }
        public string ResultSuccess { get; set; }
        public string ResultFailure { get; set; }
        public IEnumerable<ExpressionRowModel> Expressions { get; set; }
        public ExpressionRowModel RootExpression { get; set; }
        //public IEnumerable<ExpressionRowModel> ValueExpressions { get; set; }
        //public ExpressionRowModel ValueRootExpression { get; set; }
        public bool IsResultSuccessElement { get; set; }
        public bool IsResultFailureElement { get; set; }
        public string Message { get; set; }

        public string UIElementFormName { get; set; }
        public string UIElementName { get; set; }
        public int UIElementTypeID { get; set; }
        public string UIElementFullName { get; set; }
        public bool IsParentRepeater { get; set; }
        //public string ParentRepeaterType { get; set; }
        //public bool RunForRow { get; set; }
        //public bool RunForParentRow { get; set; }
        public string SuccessValueFullName { get; set; }
        public string FailureValueFullName { get; set; }
    }
}
