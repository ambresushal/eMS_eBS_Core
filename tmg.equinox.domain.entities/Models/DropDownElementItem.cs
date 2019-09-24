using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class DropDownElementItem : Entity
    {
        public int ItemID { get; set; }
        public int UIElementID { get; set; }
        public string Value { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public string DisplayText { get; set; }
        public Nullable<int> Sequence { get; set; }
        public virtual DropDownUIElement DropDownUIElement { get; set; }

        public DropDownElementItem Clone(string username, DateTime addedBy)
        {
            DropDownElementItem element = new DropDownElementItem();
            element.ItemID = this.ItemID;
            element.Value = this.Value;
            element.AddedBy = username;
            element.AddedDate = addedBy;
            element.IsActive = this.IsActive;
            element.DisplayText = this.DisplayText;
            element.Sequence = this.Sequence;

            return element;
        }
    }
}
