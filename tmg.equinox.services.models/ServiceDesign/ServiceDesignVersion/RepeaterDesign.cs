using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.applicationservices.viewmodels.ServiceDesign.ServiceDesignVersion
{
    public class RepeaterDesign
    {
        public int ID { get; set; }
        public string DisplayName { get; set; }
        public string UIElementFullName { get; set; }
        public string GeneratedName { get; set; }
        public int Sequence { get; set; }
        public int UIElementID { get; set; }
        public List<ElementDesign> Elements { get; set; }
        public DataSourceDesign PrimaryDataSource { get; set; }
        public List<DataSourceDesign> ChildDataSources { get; set; }
        public List<DataSourceDesign> InlineDataSources { get; set; }

        internal void BuildDefaultJsonDataObject(ref StringBuilder jsonBuilder)
        {
            if (this.Elements.Count > 0)
            {
                var elementList = from elem in this.Elements where elem.IsPrimary == true select elem;
                bool hasPrimary = false;
                if (elementList != null)
                {
                    int index = 0;
                    foreach (var elem in elementList)
                    {
                        if (!String.IsNullOrEmpty(elem.GeneratedName) && IsPrimaryElement(elem))
                        {
                            elem.BuildDefaultJsonDataObject(ref jsonBuilder);
                            if (index < (elementList.Count() - 1))
                            {
                                jsonBuilder.Append(" , ");
                            }
                        }
                        index++;
                    }
                    if (index > 0)
                    {
                        hasPrimary = true;
                    }
                }
                if (this.ChildDataSources != null)
                {
                    int idx = 0;
                    if (hasPrimary == true)
                    {
                        jsonBuilder.Append(" , ");
                    }
                    foreach (var cds in this.ChildDataSources)
                    {
                        jsonBuilder.Append("\"" + cds.DataSourceName + "\": ");
                        jsonBuilder.Append(" [{ ");
                        int mapIdx = 0;
                        foreach (var map in cds.Mappings)
                        {
                            jsonBuilder.Append("\"" + map.TargetElement + "\": \"\"");
                            if (mapIdx < cds.Mappings.Count() - 1)
                            {
                                jsonBuilder.Append(" , ");
                            }
                            mapIdx++;
                        }
                        jsonBuilder.Append(" }] ");
                        if (idx < this.ChildDataSources.Count() - 1)
                        {
                            jsonBuilder.Append(" , ");
                        }
                        idx++;
                    }
                }
                if (this.InlineDataSources != null)
                {
                    int idx = 0;
                    if (hasPrimary == true)
                    {
                        jsonBuilder.Append(" , ");
                    }
                    foreach (var cds in this.InlineDataSources)
                    {
                        jsonBuilder.Append("\"" + cds.DataSourceName + "\": ");
                        jsonBuilder.Append(" [{ ");
                        int mapIdx = 0;
                        foreach (var map in cds.Mappings)
                        {
                            jsonBuilder.Append("\"" + map.TargetElement + "\": \"\"");
                            if (mapIdx < cds.Mappings.Count() - 1)
                            {
                                jsonBuilder.Append(" , ");
                            }
                            mapIdx++;
                        }
                        jsonBuilder.Append(" }] ");
                        if (idx < this.InlineDataSources.Count() - 1)
                        {
                            jsonBuilder.Append(" , ");
                        }
                        idx++;
                    }
                }
            }
        }

        public bool IsPrimaryElement(ElementDesign element)
        {
            bool isPrimary = true;
            if (this.ChildDataSources != null)
            {
                foreach (var cds in this.ChildDataSources)
                {
                    var elem = (from el in cds.Mappings where el.TargetElement == element.GeneratedName select el.TargetElement).FirstOrDefault();
                    if (!String.IsNullOrEmpty(elem))
                    {
                        isPrimary = false;
                        break;
                    }
                }
            }
            return isPrimary;
        }
    }
}
