using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class ExpressionGu :Entity
    {
       public ExpressionGu()
        {
            this.Expression1 = new List<ExpressionGu>();
        }

        [Key]
        public int ExpressionID { get; set; }
        public string LeftOperand { get; set; }
        public string RightOperand { get; set; }
        public int OperatorTypeID { get; set; }
        public int LogicalOperatorTypeID { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public int RuleID { get; set; }
        public Nullable<int> ParentExpressionID { get; set; }
        public Nullable<int> ExpressionTypeID { get; set; }
        public bool IsRightOperandElement { get; set; }
        public virtual ICollection<ExpressionGu> Expression1 { get; set; }
        public virtual ExpressionGu Expression2 { get; set; }
        public virtual ExpressionType ExpressionType { get; set; }
        public virtual LogicalOperatorType LogicalOperatorType { get; set; }
        public virtual OperatorType OperatorType { get; set; }
        public virtual RuleGu Rule { get; set; }
    }
}
