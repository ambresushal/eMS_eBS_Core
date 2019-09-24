using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class FormDesignVersion : Entity
    {
        public FormDesignVersion()
        {
            this.FormInstances = new List<FormInstance>();
            this.DataSources = new List<DataSource>();
            this.DataSourceMappings = new List<DataSourceMapping>();
            this.FormVersionObjectVersionMaps = new List<FormVersionObjectVersionMap>();
            this.FormDesignDataPaths = new List<FormDesignDataPath>();
            this.FormInstanceActivityLogs = new List<FormInstanceActivityLog>();
            this.ServiceVersions = new List<ServiceDesignVersion>();
            this.FormDesignElementValues = new List<FormDesignElementValue>();
        }

        public int FormDesignVersionID { get; set; }
        public Nullable<int> FormDesignID { get; set; }
        public Nullable<int> TenantID { get; set; }
        public string VersionNumber { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public string FormDesignVersionData { get; set; }
        public int StatusID { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string Comments { get; set; }
        public virtual ICollection<FormInstance> FormInstances { get; set; }
        public virtual ICollection<DataSource> DataSources { get; set; }
        public virtual ICollection<DataSourceMapping> DataSourceMappings { get; set; }
        public virtual FormDesign FormDesign { get; set; }
        public virtual Status Status { get; set; }
        public virtual ICollection<FormVersionObjectVersionMap> FormVersionObjectVersionMaps { get; set; }
        public virtual ICollection<FormDesignDataPath> FormDesignDataPaths { get; set; }
        public virtual ICollection<FormDesignAccountPropertyMap> FormDesignAccountPropertyMaps { get; set; }
        public virtual ICollection<FormReportVersionMap> FormReportVersionMaps { get; set; }
        public Nullable<int> FormDesignTypeID { get; set; }
        public virtual FormDesignType FormDesignType { get; set; }
        public virtual ICollection<Template> Template { get; set; }
        public virtual ICollection<FormInstanceActivityLog> FormInstanceActivityLogs { get; set; }

        public virtual ICollection<AlternateUIElementLabel> AlternateUIElementLabels { get; set; }
        public virtual ICollection<TemplateReportFormDesignVersionMap> TemplateReportFormDesignVersionMaps { get; set; }
        public virtual ICollection<ServiceDesignVersion> ServiceVersions { get; set; }
        public virtual ICollection<FormDesignElementValue> FormDesignElementValues { get; set; }
        public virtual ICollection<DocumentRule> DocumentRule { get; set; }
        public string RuleExecutionTreeJSON { get; set; }
        public string RuleEventMapJSON { get; set; }

        public string PBPViewImpacts { get; set; }
        //public virtual ICollection<DPFPlanTaskUserMapping> DPFPlanTaskUserMappings { get; set; }
    }
}
