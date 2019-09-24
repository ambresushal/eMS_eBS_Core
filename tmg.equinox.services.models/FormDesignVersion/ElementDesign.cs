using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
namespace tmg.equinox.applicationservices.viewmodels.FormDesignVersion
{
    public class ElementDesign
    {
        public int ElementID { get; set; }
        public string Name { get; set; }
        public string GeneratedName { get; set; }
        public string FullName { get; set; }
        public string Label { get; set; }
        public string AlternateLabel { get; set; }
        public bool IsLabel { get; set; }
        public bool Multiline { get; set; }
        public bool MultiSelect { get; set; }
        public string HelpText { get; set; }
        public bool Enabled { get; set; }
        public string Type { get; set; }
        public string DataType { get; set; }
        public int DataTypeID { get; set; }
        public string Value { get; set; }
        public bool SpellCheck { get; set; }
        public int MaxLength { get; set; }
        public string DefaultValue { get; set; }
        public List<Item> Items { get; set; }
        public string OptionLabel { get; set; }
        public string OptionLabelNo { get; set; }
        public RepeaterDesign Repeater { get; set; }
        public SectionDesign Section { get; set; }
        public bool HasCustomRule { get; set; }
        public bool IsMatch { get; set; }
        public bool Visible { get; set; }
        public bool IsPrimary { get; set; }
        public List<int> RulesToExecute { get; set; }
        public ValidationDesign Validation { get; set; }
        public DuplicationDesign Duplication { get; set; }
        public DateTime? MaxDate { get; set; }
        public DateTime? MinDate { get; set; }
        public bool? IsDropDownTextBox { get; set; }
        public bool IsDropDownFilterable { get; set; }
        public bool? IsSortRequired { get; set; }
        public bool CheckDuplicate { get; set; }
        public bool IsKey { get; set; }
        public bool IsRichTextBox { get; set; }
        public string Layout { get; set; }
        public string EffDt { get; set; }
        public string Op { get; set; }
        public bool IsSameSectionRuleSource { get; set; }
        public string MDMName { get; set; }

        internal void BuildDefaultJSONDataObject(ref StringBuilder jsonBuilder, bool isNewFormInstance)
        {
            if (this.Section != null)
            {
                this.Section.BuildDefaultJSONDataObject(ref jsonBuilder, isNewFormInstance);
            }
            else
            {
                jsonBuilder.Append("\"" + this.GeneratedName + "\": ");

                if (this.Repeater != null)
                {
                    if (isNewFormInstance == true)
                    {
                        JArray empty = new JArray();
                        jsonBuilder.Append(empty);
                    }
                    else
                    {
                        jsonBuilder.Append(" [{ ");
                        this.Repeater.BuildDefaultJSONDataObject(ref jsonBuilder, isNewFormInstance);
                        jsonBuilder.Append(" }] ");
                    }
                }
                else
                {
                    if (this.Type == "checkbox" || this.Type == "radio")
                    {
                        var defaultVal = this.DefaultValue == null || this.DefaultValue == "False"
                                             ? null
                                             : this.DefaultValue;

                        jsonBuilder.Append(" \"" + defaultVal + "\" ");
                    }
                    else
                    {
                        jsonBuilder.Append(" \"" + this.DefaultValue + "\" ");
                    }
                }
            }
        }

        internal void BuildSyncMacro(ref StringBuilder jsonBuilder, bool isNewFormInstance)
        {
            if (this.Section != null)
            {
                this.Section.BuildSyncMacro(ref jsonBuilder, isNewFormInstance);
            }
            else
            {
                jsonBuilder.Append("\"" + this.GeneratedName + "\" : ");

                if (this.Repeater != null)
                {
                    if (isNewFormInstance == true)
                    {
                        JArray empty = new JArray();
                        jsonBuilder.Append(empty);
                    }
                    else
                    {
                        jsonBuilder.Append("{\"Fields\" : [] , \"Keys\" : [");
                        this.Repeater.BuildSyncMacro(ref jsonBuilder, isNewFormInstance);
                        jsonBuilder.Append(" ]} ");
                    }
                }
            }
        }
    }
}
