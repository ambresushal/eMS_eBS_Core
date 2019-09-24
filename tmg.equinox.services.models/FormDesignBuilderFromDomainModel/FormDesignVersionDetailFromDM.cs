using System.Collections.Generic;
using System.Text;

namespace tmg.equinox.applicationservices.viewmodels.FormDesignBuilderFromDomainModel
{
    public class FormDesignVersionDetailFromDM
    {
        public int TenantID { get; set; }
        public int FormDesignVersionId { get; set; }
        public int FormDesignId { get; set; }
        public string FormVersion { get; set; }
        public string FormName { get; set; }
        public List<SectionDesignFromDM> Sections { get; set; }
        //public List<DataSourceDesign> DataSources { get; set; }
        //public List<RuleDesign> Rules { get; set; }
        public string JSONData { get; set; }

        public string GetDefaultJSONDataObject()
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append('{');
            for (int index = 0; index < this.Sections.Count; index++)
            {
                this.Sections[index].BuildDefaultJSONDataObject(ref jsonBuilder);
                if (index < (this.Sections.Count - 1))
                {
                    jsonBuilder.Append(',');
                }
            }
            jsonBuilder.Append('}');
            return jsonBuilder.ToString();
        }
    }
}
