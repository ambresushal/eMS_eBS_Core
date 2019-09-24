using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class DropDownUIElement : UIElement
    {
        public DropDownUIElement()
        {
            this.DropDownElementItems = new List<DropDownElementItem>();
        }

        public int UIElementTypeID { get; set; }
        public string SelectedValue { get; set; }
        public bool IsMultiSelect { get; set; }
        public bool IsDropDownFilterable { get; set; }
        public Nullable<bool> IsDropDownTextBox { get; set; }
        public Nullable<bool> IsSortRequired { get; set; }
        public virtual ICollection<DropDownElementItem> DropDownElementItems { get; set; }
        //public virtual UIElement UIElement { get; set; }
        public virtual UIElementType UIElementType { get; set; }

        public DropDownUIElement Clone(string username, DateTime addedDate)
        {
            DropDownUIElement element = new DropDownUIElement();

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

            element.UIElementTypeID = this.UIElementTypeID;
            element.SelectedValue = this.SelectedValue;
            element.IsMultiSelect = this.IsMultiSelect;
            element.IsDropDownTextBox = this.IsDropDownTextBox;
            element.IsDropDownFilterable = this.IsDropDownFilterable;
            element.IsSortRequired = this.IsSortRequired;

            element.DropDownElementItems = new List<DropDownElementItem>();
            foreach (var item in this.DropDownElementItems)
            {
                element.DropDownElementItems.Add(item.Clone(username, addedDate));
            }

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
