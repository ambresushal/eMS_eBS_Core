using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FormDesignVersion
{
    public class ExpressionDesign
    {
        public int ExpressionId { get; set; }
        public string RightOperand { get; set; }
        public string RightOperandName { get; set; }
        public bool IsRightOperandElement { get; set; }
        public int OperatorTypeId { get; set; }
        public string LeftOperand { get; set; }
        public string LeftOperandName { get; set; }
        public int LogicalOperatorTypeId { get; set; }
        public List<RepeaterKeyFilterDesign> LeftKeyFilters { get; set; }
        public List<RepeaterKeyFilterDesign> RightKeyFilters { get; set; }
        public ComplexOperatorDesign complexOp { get; set; }
        public Nullable<int> ExpressionTypeId { get; set; }
        public Nullable<int> ParentExpressionId { get; set; }
        public List<ExpressionDesign> Expressions { get; set; }

    }

    public class RepeaterKeyFilterDesign
    {
        public string RepeaterKey { get; set; }
        public string RepeaterKeyName { get; set; }
        public string RepeaterKeyValue { get; set; }
    }

    public class ComplexOperatorDesign
    {
        public string Factor { get; set; }
        public decimal FactorValue { get; set; }
    }
}
