using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.GlobalUpdate
{
    public class ExpressionViewModel : ViewModelBase
    {
        #region Instance Properties
        public Int32 ExpressionID { get; set; }
        public Int32 RuleID { get; set; }
        public Int32? ParentExpressionID { get; set; }
        public Int32 ExpressionTypeID { get; set; }
        public String LeftOperand { get; set; }
        public String RightOperand { get; set; }
        public Int32 OperatorTypeID { get; set; }
        public Int32 LogicalOperatorTypeID { get; set; }
        public Boolean? IsRightOperandElement { get; set; }
        #endregion Instance Properties
    }
}
