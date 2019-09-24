using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class RuleTargetType : Entity
    {
        public RuleTargetType()
        {
            this.Rules = new List<Rule>();
            this.RulesGu = new List<RuleGu>();
        }

        public int RuleTargetTypeID { get; set; }
        public string RuleTargetTypeCode { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public string DisplayText { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Rule> Rules { get; set; }
        public virtual ICollection<RuleGu> RulesGu { get; set; }
    }
}
