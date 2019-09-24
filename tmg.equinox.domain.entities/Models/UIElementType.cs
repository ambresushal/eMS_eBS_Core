using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class UIElementType : Entity
    {
        public UIElementType()
        {
            this.CalendarUIElements = new List<CalendarUIElement>();
            this.CheckBoxUIElements = new List<CheckBoxUIElement>();
            this.DropDownUIElements = new List<DropDownUIElement>();
            this.RadioButtonUIElements = new List<RadioButtonUIElement>();
            this.SectionUIElements = new List<SectionUIElement>();
            this.TabUIElements = new List<TabUIElement>();
            this.TextBoxUIElements = new List<TextBoxUIElement>();
            this.ServiceDefinitions = new List<ServiceDefinition>();
            this.IASElementExports = new List<IASElementExport>();
        }

        public int UIElementTypeID { get; set; }
        public string UIElementTypeCode { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public string DisplayText { get; set; }
        public string Description { get; set; }
        public virtual ICollection<CalendarUIElement> CalendarUIElements { get; set; }
        public virtual ICollection<CheckBoxUIElement> CheckBoxUIElements { get; set; }
        public virtual ICollection<DropDownUIElement> DropDownUIElements { get; set; }
        public virtual ICollection<RadioButtonUIElement> RadioButtonUIElements { get; set; }
        public virtual ICollection<SectionUIElement> SectionUIElements { get; set; }
        public virtual ICollection<TabUIElement> TabUIElements { get; set; }
        public virtual ICollection<TextBoxUIElement> TextBoxUIElements { get; set; }
        public virtual ICollection<ServiceDefinition> ServiceDefinitions { get; set; }
        public virtual ICollection<IASElementExport> IASElementExports { get; set; }

        public virtual ICollection<IASElementImport> IASElementImports { get; set; }
    }
}
