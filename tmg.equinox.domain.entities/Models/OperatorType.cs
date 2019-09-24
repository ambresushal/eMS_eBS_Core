using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class OperatorType : Entity
    {
        public OperatorType()
        {
            this.Expressions = new List<Expression>();
            this.ExpressionsGu = new List<ExpressionGu>();
        }

        public int OperatorTypeID { get; set; }
        public string OperatorTypeCode { get; set; }
        public string DisplayText { get; set; }
        public string Description { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Expression> Expressions { get; set; }
        public virtual ICollection<ExpressionGu> ExpressionsGu { get; set; }
    }
}
