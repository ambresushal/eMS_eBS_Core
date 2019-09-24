using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class FolderVersion : Entity
    {
        public FolderVersion()
        {
            this.AccountProductMaps = new List<AccountProductMap>();
            this.FolderVersionWorkFlowStates = new List<FolderVersionWorkFlowState>();
            this.FormInstances = new List<FormInstance>();
            this.WorkFlowStateUserMaps = new List<WorkFlowStateUserMap>();
            this.WorkFlowStateFolderVersionMaps = new List<WorkFlowStateFolderVersionMap>();
            this.EmailLogs = new List<EmailLog>();
            this.FormInstanceActivityLogs = new List<FormInstanceActivityLog>();
        }

        public int FolderVersionID { get; set; }
        public int FolderID { get; set; }
        public System.DateTime EffectiveDate { get; set; }
        public Nullable<int> WFStateID { get; set; }
        public string FolderVersionNumber { get; set; }
        public int VersionTypeID { get; set; }
        public bool IsActive { get; set; }
        public int TenantID { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string Comments { get; set; }
        public int FolderVersionStateID { get; set; }
        public Nullable<int> FolderVersionBatchID { get; set; }
        public Nullable<int> ConsortiumID { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public string CatID { get; set; }
        public virtual ICollection<AccountProductMap> AccountProductMaps { get; set; }
        public virtual Folder Folder { get; set; }
        public virtual FolderVersionBatch FolderVersionBatch { get; set; }
        public virtual FolderVersionState FolderVersionState { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual VersionType VersionType { get; set; }
        public virtual WorkFlowVersionState WorkFlowVersionState { get; set; }
        public virtual Consortium Consortium { get; set; }
        //public virtual FolderVersionCategory FolderVersionCategory { get; set; }
        public virtual ICollection<FolderVersionWorkFlowState> FolderVersionWorkFlowStates { get; set; }
        public virtual ICollection<FormInstance> FormInstances { get; set; }
        public virtual ICollection<Journal> Journals { get; set; }
        public virtual ICollection<WorkFlowStateUserMap> WorkFlowStateUserMaps { get; set; }
        public virtual ICollection<WorkFlowStateFolderVersionMap> WorkFlowStateFolderVersionMaps { get; set; }
        public virtual ICollection<EmailLog> EmailLogs { get; set; }
        public virtual ICollection<FormInstanceActivityLog> FormInstanceActivityLogs { get; set; }
        public virtual ICollection<IASFolderMap> IASFolderMaps { get; set; }
        public virtual FolderVersionCategory FolderVersionCategory { get; set; }
        public virtual ICollection<InterestedFolderVersion> InterstedFolderVersions { get; set; }
        public virtual ICollection<DPFPlanTaskUserMapping> DPFPlanTaskUserMappings { get; set; }
        public virtual ICollection<TaskComments> TaskCommentsMappings { get; set; }
    }
}
