using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class LayoutType : Entity
    {
        public LayoutType()
        {
            this.RepeaterUIElements = new List<RepeaterUIElement>();
            this.SectionUIElements = new List<SectionUIElement>();
            this.TabUIElements = new List<TabUIElement>();
        }

        public int LayoutTypeID { get; set; }
        public string Name { get; set; }
        public string LayoutTypeCode { get; set; }
        public string DisplayText { get; set; }
        public string ClassName { get; set; }
        public Nullable<int> ColumnCount { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<RepeaterUIElement> RepeaterUIElements { get; set; }
        public virtual ICollection<SectionUIElement> SectionUIElements { get; set; }
        public virtual ICollection<TabUIElement> TabUIElements { get; set; }
    }
}
