using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class Tenant : Entity
    {
        public Tenant()
        {
            this.Accounts = new List<Account>();
            this.Accounts1 = new List<Account>();
            this.AccountProductMaps = new List<AccountProductMap>();
            this.WorkFlowStateApprovalTypeMaster = new List<WorkFlowStateApprovalTypeMaster>();
            this.Folders = new List<Folder>();
            this.FolderVersions = new List<FolderVersion>();
            this.FolderVersionWorkFlowStates = new List<FolderVersionWorkFlowState>();
            this.FormInstances = new List<FormInstance>();
            this.MarketSegments = new List<MarketSegment>();
            this.VersionTypes = new List<VersionType>();
            //this.WorkFlowStates = new List<WorkFlowVersionState>();
            this.WorkFlowStateGroups = new List<WorkFlowStateGroup>();
            this.Users = new List<User>();
            this.FormDesignDataPaths = new List<FormDesignDataPath>();
            this.SBCReportServiceMasters = new List<SBCReportServiceMaster>();
            this.AutoSaveSettings = new List<AutoSaveSettings>();
            this.FolderLock = new List<FolderLock>();
            this.WorkFlowStateUserMaps = new List<WorkFlowStateUserMap>();
            this.EmailLogs = new List<EmailLog>();
            this.ServiceParameters = new List<ServiceParameter>();
            this.GlobalUpdates = new List<GlobalUpdate>();
        }

        public int TenantID { get; set; }
        public string TenantName { get; set; }
        public bool IsActive { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public System.DateTime CreateDt { get; set; }
        public Nullable<System.DateTime> UpdateDt { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Account> Accounts1 { get; set; }
        public virtual ICollection<AccountProductMap> AccountProductMaps { get; set; }
        //public virtual ICollection<WorkFlowStateApprovalTypeMaster> ApprovalStatusTypes { get; set; }
        public virtual ICollection<Folder> Folders { get; set; }
        public virtual ICollection<FolderVersion> FolderVersions { get; set; }
        public virtual ICollection<FolderVersionWorkFlowState> FolderVersionWorkFlowStates { get; set; }
        public virtual ICollection<FormInstance> FormInstances { get; set; }
        public virtual ICollection<MarketSegment> MarketSegments { get; set; }
        public virtual ICollection<VersionType> VersionTypes { get; set; }
        //public virtual ICollection<WorkFlowVersionState> WorkFlowStates { get; set; }
        public virtual ICollection<WorkFlowStateGroup> WorkFlowStateGroups { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<FormDesignDataPath> FormDesignDataPaths { get; set; }
        public virtual ICollection<SBCReportServiceMaster> SBCReportServiceMasters { get; set; }
        public virtual ICollection<FormDesignAccountPropertyMap> FormDesignAccountPropertyMaps { get; set; }
        public virtual ICollection<AutoSaveSettings> AutoSaveSettings { get; set; }
        public virtual ICollection<Template> Template { get; set; }
        public virtual ICollection<FolderLock> FolderLock { get; set; }
        public virtual ICollection<TemplateUIMap> TemplateUIMap { get; set; }
        public virtual ICollection<WorkFlowStateUserMap> WorkFlowStateUserMaps { get; set; }
        public virtual ICollection<EmailLog> EmailLogs { get; set; }
        public virtual ICollection<FormReport> FormReports { get; set; }
        public virtual ICollection<FormReportVersion> FormReportVersions { get; set; }
        public virtual ICollection<ServiceDefinition> ServiceDefinitions { get; set; }
        public virtual ICollection<ServiceDesign> ServiceDesigns { get; set; }
        public virtual ICollection<ServiceDesignVersion> ServiceDesignVersions { get; set; }
        public virtual ICollection<ServiceParameter> ServiceParameters { get; set; }
        public virtual ICollection<GlobalUpdate> GlobalUpdates { get; set; }
        public virtual ICollection<WorkFlowStateMaster> WorkFlowStateMaster { get; set; }
        public virtual ICollection<WorkFlowStateApprovalTypeMaster> WorkFlowStateApprovalTypeMaster { get; set; }
        public virtual ICollection<WorkFlowCategoryMapping> WorkFlowCategoryMapping { get; set; }
    }
}
