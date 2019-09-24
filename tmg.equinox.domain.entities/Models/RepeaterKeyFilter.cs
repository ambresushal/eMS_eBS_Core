using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class RepeaterKeyFilter : Entity
    {
        public RepeaterKeyFilter()
        {
        }

        public int RepeaterKeyID { get; set; }
        public int ExpressionID { get; set; }
        public string RepeaterKey { get; set; }
        public string RepeaterKeyValue { get; set; }
        public bool IsRightOperand { get; set; }
        public int? PropertyRuleMapID { get; set; }
        public RepeaterKeyFilter Clone(string username, DateTime addedDate)
        {
            var keyFilter = new RepeaterKeyFilter();
            keyFilter.RepeaterKeyID = this.RepeaterKeyID;
            keyFilter.ExpressionID = this.ExpressionID;
            keyFilter.RepeaterKey = this.RepeaterKey;
            keyFilter.RepeaterKeyValue = this.RepeaterKeyValue;
            keyFilter.IsRightOperand = this.IsRightOperand;

            return keyFilter;
        }
    }
}
