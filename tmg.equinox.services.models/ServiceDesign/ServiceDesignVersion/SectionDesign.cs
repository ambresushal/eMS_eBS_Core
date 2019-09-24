using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.ServiceDesign.ServiceDesignVersion
{
    public class SectionDesign
    {
        public int ID { get; set; }
        public string DisplayName { get; set; }
        public string UIElementFullName { get; set; }
        public string GeneratedName { get; set; }
        public int Sequence { get; set; }
        public int UIElementID { get; set; }
        public List<ElementDesign> Elements { get; set; }

        internal void BuildDefaultJsonDataObject(ref StringBuilder jsonBuilder)
        {
            jsonBuilder.Append("\"" + this.GeneratedName + "\": { ");
            for (int index = 0; index < this.Elements.Count; index++)
            {
                if (!String.IsNullOrEmpty(this.Elements[index].GeneratedName))
                {
                    this.Elements[index].BuildDefaultJsonDataObject(ref jsonBuilder);
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
