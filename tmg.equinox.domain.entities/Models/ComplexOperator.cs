using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class ComplexOperator : Entity
    {
        public ComplexOperator()
        {
        }

        public int ComplexOperatorID { get; set; }
        public int ExpressionID { get; set; }
        public int OperatorID { get; set; }
        public string Factor { get; set; }
        public decimal FactorValue { get; set; }

        public ComplexOperator Clone(string username, DateTime addedDate)
        {
            var comOp = new ComplexOperator();
            comOp.ComplexOperatorID = this.ComplexOperatorID;
            comOp.ExpressionID = this.ExpressionID;
            comOp.OperatorID = this.OperatorID;
            comOp.Factor = this.Factor;
            comOp.FactorValue = this.FactorValue;

            return comOp;
        }
    }
}
