using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class RuleGu : Entity
    {
        public RuleGu()
        {
            this.ExpressionsGu = new List<ExpressionGu>();
        }
        [Key]
        public int RuleID { get; set; }
        public string RuleName { get; set; }
        public string RuleDescription { get; set; }
        public int RuleTargetTypeID { get; set; }
        public string ResultSuccess { get; set; }
        public string ResultFailure { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public virtual ICollection<ExpressionGu> ExpressionsGu { get; set; }
        public bool IsResultSuccessElement { get; set; }
        public bool IsResultFailureElement { get; set; }
        public string Message { get; set; }
        public int TargetPropertyID { get; set; }
        public int UIElementID { get; set; }
        public int GlobalUpdateID { get; set; }
        public virtual RuleTargetType RuleTargetType { get; set; }
        public virtual TargetProperty TargetProperty { get; set; }
        public virtual UIElement UIElement { get; set; }
        public virtual GlobalUpdate GlobalUpdate { get; set; }
    }
}

