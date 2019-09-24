using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class TargetRepeaterKeyFilter : Entity
    {
        public TargetRepeaterKeyFilter()
        {

        }

        public int TargetRepeaterKeyID { get; set; }
        public int RuleID { get; set; }
        public string RepeaterKey { get; set; }
        public string RepeaterKeyValue { get; set; }
        public int? PropertyRuleMapID { get; set; }
        public TargetRepeaterKeyFilter Clone(string username, DateTime addedDate)
        {
            var keyFilter = new TargetRepeaterKeyFilter();
            keyFilter.TargetRepeaterKeyID = this.TargetRepeaterKeyID;
            keyFilter.RuleID = this.RuleID;
            keyFilter.RepeaterKey = this.RepeaterKey;
            keyFilter.RepeaterKeyValue = this.RepeaterKeyValue;

            return keyFilter;
        }
    }
}
