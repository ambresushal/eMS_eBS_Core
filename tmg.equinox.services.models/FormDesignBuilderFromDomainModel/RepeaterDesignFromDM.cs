using System;
using System.Collections.Generic;
using System.Text;

namespace tmg.equinox.applicationservices.viewmodels.FormDesignBuilderFromDomainModel
{
    public class RepeaterDesignFromDM
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string GeneratedName { get; set; }
        public string FullName { get; set; }
        public string Label { get; set; }
        public string HelpText { get; set; }
        public string RepeaterType { get; set; }
        public int? LayoutColumn { get; set; }
        public string LayoutClass { get; set; }
        public int RowCount { get; set; }
        public List<ElementDesignFromDM> Elements { get; set; }
        public int ChildCount { get; set; }

        internal void BuildDefaultJSONDataObject(ref StringBuilder jsonBuilder)
        {
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
        }
    }
}
