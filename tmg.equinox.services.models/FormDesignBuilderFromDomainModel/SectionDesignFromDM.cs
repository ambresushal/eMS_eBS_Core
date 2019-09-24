using System;
using System.Collections.Generic;
using System.Text;

namespace tmg.equinox.applicationservices.viewmodels.FormDesignBuilderFromDomainModel
{
    public class SectionDesignFromDM
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string GeneratedName { get; set; }
        public string FullName { get; set; }
        public string Label { get; set; }
        public string HelpText { get; set; }
        public int Sequence { get; set; }
        public int ChildCount { get; set; }
        public bool Visible { get; set; }
        public bool Enabled { get; set; }
        public int? LayoutColumn { get; set; }
        public string LayoutClass { get; set; }
        public List<ElementDesignFromDM> Elements { get; set; }

        internal void BuildDefaultJSONDataObject(ref StringBuilder jsonBuilder)
        {
            jsonBuilder.Append("\"" + this.GeneratedName + "\": { ");
            for (int index = 0; index < this.Elements.Count; index++)
            {
                if (!String.IsNullOrEmpty(this.Elements[index].GeneratedName))
                {
                    this.Elements[index].BuildDefaultJSONDataObject(ref jsonBuilder);
                    if (index < (this.Elements.Count - 1))
                    {
                        jsonBuilder.Append(" , ");
                    }
                }
            }
            jsonBuilder.Append('}');
        }
    }
}
