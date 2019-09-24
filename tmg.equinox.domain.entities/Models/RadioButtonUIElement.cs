using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class RadioButtonUIElement : UIElement
    {
        public int UIElementTypeID { get; set; }
        public string OptionLabel { get; set; }
        public Nullable<bool> DefaultValue { get; set; }
        public bool IsYesNo { get; set; }
        public string OptionLabelNo { get; set; }
        //public virtual UIElement UIElement { get; set; }
        public virtual UIElementType UIElementType { get; set; }

        public RadioButtonUIElement Clone(string username, DateTime addedDate)
        {
            RadioButtonUIElement element = new RadioButtonUIElement();
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
            element.UIElementID = this.UIElementID;
            element.UIElementTypeID = this.UIElementTypeID;
            element.OptionLabel = this.OptionLabel;
            element.DefaultValue = this.DefaultValue;
            element.IsYesNo = this.IsYesNo;
            element.OptionLabelNo = this.OptionLabelNo;

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
