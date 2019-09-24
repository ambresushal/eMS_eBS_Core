using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class TargetProperty : Entity
    {
        public TargetProperty()
        {
            this.PropertyRuleMaps = new List<PropertyRuleMap>();
            this.RulesGu = new List<RuleGu>();
        }

        public int TargetPropertyID { get; set; }
        public string TargetPropertyName { get; set; }
        public virtual ICollection<PropertyRuleMap> PropertyRuleMaps { get; set; }
        public virtual ICollection<RuleGu> RulesGu { get; set; }
    }
}
