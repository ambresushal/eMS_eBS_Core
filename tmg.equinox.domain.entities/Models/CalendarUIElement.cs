using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class CalendarUIElement : UIElement
    {
        public int UIElementTypeID { get; set; }
        public Nullable<System.DateTime> MinDate { get; set; }
        public Nullable<System.DateTime> MaxDate { get; set; }
        public Nullable<System.DateTime> DefaultDate { get; set; }
        //public virtual UIElement UIElement { get; set; }
        public virtual UIElementType UIElementType { get; set; }

        public CalendarUIElement Clone(string username, DateTime addedDate)
        {
            CalendarUIElement element = new CalendarUIElement();

            element.UIElementName = this.UIElementName;
            element.Label = this.Label;
            element.ParentUIElementID = this.ParentUIElementID;
            element.IsContainer = this.IsContainer;
            element.Enabled = this.Enabled;
            element.Visible = this.Visible;
            element.Sequence = this.Sequence;
            element.RequiresValidation = this.RequiresValidation;
            element.HelpText = this.HelpText;
            element.AddedBy = username;
            element.AddedDate = addedDate;
            element.IsActive = this.IsActive;
            element.FormID = this.FormID;
            element.UIElementDataTypeID = this.UIElementDataTypeID;
            element.HasCustomRule = this.HasCustomRule;
            element.CustomRule = this.CustomRule;
            element.GeneratedName = this.GeneratedName;
            element.UIElementTypeID = this.UIElementTypeID;

            element.MinDate = this.MinDate;
            element.MaxDate = this.MaxDate;
            element.DefaultDate = this.DefaultDate;

            element.Validators = new List<Validator>();
            foreach (var item in this.Validators)
            {
                element.Validators.Add(item.Clone(username, addedDate));
            }

            element.PropertyRuleMaps = new List<PropertyRuleMap>();
            foreach (var item in this.PropertyRuleMaps)
            {
                element.PropertyRuleMaps.Add(item.Clone(username, addedDate));
            }
            return element;
        }
    }
}
