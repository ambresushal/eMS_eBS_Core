using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class Expression : Entity
    {
        public Expression()
        {
            this.Expression1 = new List<Expression>();
        }

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
        public virtual ICollection<Expression> Expression1 { get; set; }
        public virtual Expression Expression2 { get; set; }
        public virtual ExpressionType ExpressionType { get; set; }
        public virtual LogicalOperatorType LogicalOperatorType { get; set; }
        public virtual OperatorType OperatorType { get; set; }
        public bool IsRightOperandElement { get; set; }
        public virtual Rule Rule { get; set; }

        public Expression Clone(string username, DateTime addedDate)
        {
            var newExp = new Expression();
            newExp.ExpressionID = this.ExpressionID;
            newExp.LeftOperand = this.LeftOperand;
            newExp.RightOperand = this.RightOperand;
            newExp.OperatorTypeID = this.OperatorTypeID;
            newExp.LogicalOperatorTypeID = this.LogicalOperatorTypeID;
            newExp.AddedBy = username;
            newExp.AddedDate = addedDate;
            newExp.RuleID = this.RuleID;
            newExp.IsRightOperandElement = this.IsRightOperandElement;

            return newExp;
        }
    }
}
