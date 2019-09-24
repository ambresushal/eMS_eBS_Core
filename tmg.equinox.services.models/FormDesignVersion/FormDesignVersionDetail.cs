using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FormDesignVersion
{
    public class FormDesignVersionDetail
    {
        public int TenantID { get; set; }
        public int FormDesignVersionId { get; set; }
        public int FormDesignId { get; set; }
        public string FormVersion { get; set; }
        public string FormName { get; set; }
        public List<SectionDesign> Sections { get; set; }
        public List<DataSourceDesign> DataSources { get; set; }
        public List<RuleDesign> Rules { get; set; }
        public List<ElementRuleMap> ElementRuleMaps { get; set; }
        public List<ValidationDesign> Validations { get; set; }
        public List<DuplicationDesign> Duplications { get; set; }
        public string JSONData { get; set; }
        public string CustomRules { get; set; }
        public bool IsNewVersionLoaded { get; set; }
        public bool IsNewLoadedVersionIsMajorVersion { get; set; }
        public bool IsNewFormInstance { get; set; }
        public string errorGridData = string.Empty;
        public bool IsMasterList { get; set; }
        public bool IsAliasDesignMasterList { get; set; }
        public bool UsesAliasDesignMasterList { get; set; }
        public bool IsNewVersionMerged { get; set; }
        public bool IsSectionLock { get; set; }

        public string GetDefaultJSONDataObject()
        {

            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append('{');
            for (int index = 0; index < this.Sections.Count; index++)
            {
                this.Sections[index].BuildDefaultJSONDataObject(ref jsonBuilder, this.IsNewFormInstance);
                if (index < (this.Sections.Count - 1))
                {
                    jsonBuilder.Append(',');
                }
            }
            jsonBuilder.Append('}');
            return jsonBuilder.ToString();
        }

        public string GetSyncMacro()
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append('{');
            for (int index = 0; index < this.Sections.Count; index++)
            {
                this.Sections[index].BuildSyncMacro(ref jsonBuilder, this.IsNewFormInstance);
                if (index < (this.Sections.Count - 1))
                {
                    jsonBuilder.Append(',');
                }
            }
            jsonBuilder.Append('}');
            return jsonBuilder.ToString();
        }
    }

    public class SectionDetails
    {
        public SectionDesign SectionDetail { get; set; }
        public List<DataSourceDesign> DataSource { get; set; }
        public string SectionData { get; set; }
    }
}
