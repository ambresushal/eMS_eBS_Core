using System;
using System.Collections.Generic;

namespace tmg.equinox.integration.data.Models
{
    public partial class FolderVersion : Entity
    {
        public FolderVersion()
        {
            //this.AccountProductMaps = new List<AccountProductMap>();
            //this.FolderVersionWorkFlowStates = new List<FolderVersionWorkFlowState>();
            this.FormInstances = new List<FormInstance>();
        }

        public int FolderVersionID { get; set; }
        public int FolderID { get; set; }
        public System.DateTime EffectiveDate { get; set; }
        public int WFStateID { get; set; }
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
        //public virtual ICollection<AccountProductMap> AccountProductMaps { get; set; }
        public virtual Folder Folder { get; set; }
        //public virtual FolderVersionBatch FolderVersionBatch { get; set; }
        //public virtual FolderVersion FolderVersion1 { get; set; }
        //public virtual FolderVersion FolderVersion2 { get; set; }
        //public virtual Tenant Tenant { get; set; }
        //public virtual VersionType VersionType { get; set; }
        //public virtual WorkFlowState WorkFlowState { get; set; }
        //public virtual ICollection<FolderVersionWorkFlowState> FolderVersionWorkFlowStates { get; set; }
        public virtual ICollection<FormInstance> FormInstances { get; set; }
    }
}
