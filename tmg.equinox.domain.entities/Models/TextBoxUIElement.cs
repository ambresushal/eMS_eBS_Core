using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class TextBoxUIElement : UIElement
    {
        public int UIElementTypeID { get; set; }
        public Nullable<bool> IsMultiline { get; set; }
        public string DefaultValue { get; set; }
        public int MaxLength { get; set; }
        public bool IsLabel { get; set; }
        public Nullable<bool> SpellCheck { get; set; }
        //public virtual UIElement UIElement { get; set; }
        public virtual UIElementType UIElementType { get; set; }

        public TextBoxUIElement Clone(string username, DateTime addedDate)
        {
            TextBoxUIElement element = new TextBoxUIElement();

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
            element.IsMultiline = this.IsMultiline;
            element.DefaultValue = this.DefaultValue;
            element.MaxLength = this.MaxLength;
            element.IsLabel = this.IsLabel;
            element.SpellCheck = this.SpellCheck;
            element.AllowGlobalUpdates = this.AllowGlobalUpdates;

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
