using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign.ServiceDesignVersion;

namespace tmg.equinox.applicationservices.viewmodels.ServiceDesign
{
    public class ServiceDesignVersionDetail
    {
        #region Public Properties
        public int TenantID { get; set; }
        public int ServiceDesignID { get; set; }
        public int ServiceDesignVersionID { get; set; }
        public string ServiceMethodName { get; set; }
        public string VersionNumber { get; set; }
        public List<SectionDesign> Sections { get; set; }
        public List<RepeaterDesign> Repeaters { get; set; }
        public List<DataSourceDesign> DataSources { get; set; }
        public string JsonData { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }
        public string FormDesignName { get; set; }
        public ResponseType ResponseType { get; set; }
        #endregion Public Properties

        public string GetJsonDataObject()
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append('{');
            if (this.Sections.Count() > 0)
            {
                for (int index = 0; index < this.Sections.Count; index++)
                {
                    this.Sections[index].BuildDefaultJsonDataObject(ref jsonBuilder);
                    if (index < (this.Sections.Count - 1))
                    {
                        jsonBuilder.Append(',');
                    }
                }
            }

            if (this.Repeaters.Count() > 0)
            {
                for (int index = 0; index < this.Repeaters.Count; index++)
                {
                    jsonBuilder.Append("\"" + this.Repeaters[index].GeneratedName + "\": ");
                    jsonBuilder.Append(" [{ ");
                    this.Repeaters[index].BuildDefaultJsonDataObject(ref jsonBuilder);
                    jsonBuilder.Append(" }] ");
                    if (index < (this.Repeaters.Count - 1))
                    {
                        jsonBuilder.Append(',');
                    }
                }
            }

            jsonBuilder.Append('}');
            return jsonBuilder.ToString();
        }
    }
}
