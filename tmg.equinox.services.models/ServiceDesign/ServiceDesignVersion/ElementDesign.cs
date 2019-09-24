using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.applicationservices.viewmodels.ServiceDesign.ServiceDesignVersion
{
    public enum ElementDataSourceMode
    {
        Primary,
        Inline,
        Child
    }

    public class ElementDesign
    {
        public int ID { get; set; }
        public string DisplayName { get; set; }
        public string UIElementFullName { get; set; }
        public string GeneratedName { get; set; }
        public int Sequence { get; set; }
        public int UIElementID { get; set; }
        public SectionDesign Section { get; set; }
        public RepeaterDesign Repeater { get; set; }
        public string DataType { get; set; }
        public int DataTypeID { get; set; }
        public string Type { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsKey { get; set; }
        public bool IsRequired { get; set; }
        public ElementDataSourceMode DataSourceMode { get; set; }
        public string Value { get; set; }
        public bool IsSameSectionRuleSource { get; set; }

        internal void BuildDefaultJsonDataObject(ref StringBuilder jsonBuilder)
        {
            if (this.Section != null)
            {
                this.Section.BuildDefaultJsonDataObject(ref jsonBuilder);
            }
            else
            {
                jsonBuilder.Append("\"" + this.GeneratedName + "\": ");

                if (this.Repeater != null)
                {
                    jsonBuilder.Append(" [{ ");
                    this.Repeater.BuildDefaultJsonDataObject(ref jsonBuilder);
                    jsonBuilder.Append(" }] ");
                }
                else
                {
                    string value = this.Value ?? string.Empty;
                    jsonBuilder.Append(" \"" + value + "\" ");
                }
            }
        }
    }
}