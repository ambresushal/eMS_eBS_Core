using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class FormDesign : Entity
    {
        public FormDesign()
        {
            this.FormInstances = new List<FormInstance>();
            this.DataSources = new List<DataSource>();
            this.DataSourceMappings = new List<DataSourceMapping>();
            this.FormDesignGroupMappings = new List<FormDesignGroupMapping>();
            this.FormDesignVersions = new List<FormDesignVersion>();
            this.UIElements = new List<UIElement>();
            this.FormDesignDataPaths = new List<FormDesignDataPath>();
            this.FormInstanceActivityLogs = new List<FormInstanceActivityLog>();
            this.ServiceVersions = new List<ServiceDesignVersion>();
            this.FormDesignElementValues = new List<FormDesignElementValue>();
        }

        public int FormID { get; set; }
        public string FormName { get; set; }
        public string DisplayText { get; set; }
        public bool IsActive { get; set; }
        public string Abbreviation { get; set; }
        public int TenantID { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsMasterList { get; set; }
        public int DocumentDesignTypeID { get; set; }
        public int DocumentLocationID { get; set; }
        public bool IsAliasDesignMasterList { get; set; }
        public bool UsesAliasDesignMasterList { get; set; }
        public bool IsSectionLock { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public virtual ICollection<FormInstance> FormInstances { get; set; }
        public virtual ICollection<DataSource> DataSources { get; set; }
        public virtual ICollection<DataSourceMapping> DataSourceMappings { get; set; }
        public virtual ICollection<FormDesignGroupMapping> FormDesignGroupMappings { get; set; }
        public virtual ICollection<FormDesignVersion> FormDesignVersions { get; set; }
        public virtual ICollection<UIElement> UIElements { get; set; }
        public virtual ICollection<FormDesignDataPath> FormDesignDataPaths { get; set; }
        public virtual ICollection<FormDesignAccountPropertyMap> FormDesignAccountPropertyMaps { get; set; }
        public virtual ICollection<Template> Template { get; set; }
        public virtual ICollection<FormReportVersionMap> FormReportVersionMaps { get; set; }
        public virtual ICollection<FormInstanceActivityLog> FormInstanceActivityLogs { get; set; }

        public virtual ICollection<AlternateUIElementLabel> AlternateUIElementLabels { get; set; }
        public virtual ICollection<TemplateReportFormDesignVersionMap> TemplateReportFormDesignVersionMaps { get; set; }
        public virtual ICollection<ServiceDesignVersion> ServiceVersions { get; set; }
        public virtual ICollection<FormDesignElementValue> FormDesignElementValues { get; set; }

        public virtual ICollection<DocumentRule> DocumentRule { get; set; }
    }
}
