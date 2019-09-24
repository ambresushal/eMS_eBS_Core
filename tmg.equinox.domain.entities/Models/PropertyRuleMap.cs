using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class PropertyRuleMap : Entity
    {
        public int PropertyRuleMapID { get; set; }
        public int RuleID { get; set; }
        public int UIElementID { get; set; }
        public int TargetPropertyID { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<bool> IsCustomRule { get; set; }
        public virtual Rule Rule { get; set; }
        public virtual TargetProperty TargetProperty { get; set; }
        public virtual UIElement UIElement { get; set; }

        public PropertyRuleMap Clone(string username, DateTime addedDate)
        {
            PropertyRuleMap item = new PropertyRuleMap();

            item.PropertyRuleMapID = this.PropertyRuleMapID;
            item.RuleID = this.RuleID;
            item.TargetPropertyID = this.TargetPropertyID;
            item.AddedBy = username;
            item.AddedDate = addedDate;
            item.IsCustomRule = this.IsCustomRule;
            item.Rule = this.Rule.Clone(username, addedDate);
            item.UIElementID = this.UIElementID;
                        return item;
        }
    }
}
