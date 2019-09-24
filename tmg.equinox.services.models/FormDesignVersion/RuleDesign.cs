using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FormDesignVersion
{
    public class RuleDesign
    {
        public int RuleID { get; set; }
        public int UIELementID { get; set; }
        public string UIElementFormName { get; set; }
        public string UIElementName { get; set; }
        public int UIElementTypeID { get; set; }
        public string UIElementFullName { get; set; } 
        public string SuccessValue { get; set; }
        public string FailureValue { get; set; }
        public int TargetPropertyTypeId { get; set; }
        public List<RepeaterKeyFilterDesign> TargetKeyFilters { get; set; }
        public bool IsParentRepeater { get; set; }
        public string ParentRepeaterType { get; set; }
        public bool RunForRow { get; set; }
        public bool RunForParentRow { get; set; }
        public IEnumerable<ExpressionDesign> Expressions { get; set; }
        public ExpressionDesign RootExpression { get; set; }
        public bool IsResultSuccessElement { get; set; }
        public bool IsResultFailureElement { get; set; }
        public string SuccessValueFullName { get; set; }
        public string FailureValueFullName { get; set; }
        public string Message { get; set; }
        public bool RunOnLoad { get; set; }

    }
}
