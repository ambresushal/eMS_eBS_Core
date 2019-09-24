using System.Collections.Generic;
using System.Text;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;

namespace tmg.equinox.applicationservices.viewmodels.FormDesignBuilderFromDomainModel
{
    public class ElementDesignFromDM
    {
        public int ElementID { get; set; }
        public string Name { get; set; }
        public string GeneratedName { get; set; }
        public string FullName { get; set; }
        public string Label { get; set; }
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
        public RepeaterDesignFromDM Repeater { get; set; }
        public SectionDesignFromDM Section { get; set; }
        public bool HasCustomRule { get; set; }
        public bool IsMatch { get; set; }
        public bool Visible { get; set; }
        public List<int> RulesToExecute { get; set; }
        public ValidationDesign Validation { get; set; }

        internal void BuildDefaultJSONDataObject(ref StringBuilder jsonBuilder)
        {
            if (this.Section != null)
            {
                this.Section.BuildDefaultJSONDataObject(ref jsonBuilder);
            }
            else
            {
                jsonBuilder.Append("\"" + this.GeneratedName + "\": ");

                if (this.Repeater != null)
                {
                    jsonBuilder.Append(" [{ ");
                    this.Repeater.BuildDefaultJSONDataObject(ref jsonBuilder);
                    jsonBuilder.Append(" }] ");
                }
                else
                {
                    jsonBuilder.Append(" \"" + this.DefaultValue + "\" ");
                }
            }
        }
    }
}
