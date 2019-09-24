using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.UIElement
{
    public class ExpressionRowModel
    {
        public int TenantId { get; set; }

        public int UIElementId { get; set; }

        public int RuleId { get; set; }

        public int ExpressionId { get; set; }

        public string LeftOperand { get; set; }

        public string LeftOperandName { get; set; }

        public string RightOperand { get; set; }

        public string RightOperandName { get; set; }

        public int OperatorTypeId { get; set; }

        public int LogicalOperatorTypeId { get; set; }

        public bool IsRightOperandElement { get; set; }

        public List<ExpressionRowModel> Expressions { get; set; }

        public Nullable<int> ParentExpressionId { get; set; }

        public Nullable<int> ExpressionTypeId { get; set; }
        public List<RepeaterKeyFilterModel> LeftKeyFilter { get; set; }
        public List<RepeaterKeyFilterModel> RightKeyFilter { get; set; }

        public ComplextOperatorModel CompOp { get; set; }
    }

    public class RepeaterKeyFilterModel
    {
        public int UIElementID { get; set; }
        public string Label { get; set; }
        public string UIElementPath { get; set; }
        public bool isChecked { get; set; }
        public string FilterValue { get; set; }
    }

    public class ComplextOperatorModel
    {
        public string Factor { get; set; }
        public decimal FactorValue { get; set; }
    }
}
