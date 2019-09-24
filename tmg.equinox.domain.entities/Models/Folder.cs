using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class Folder : Entity
    {
        public Folder()
        {
            this.AccountProductMaps = new List<AccountProductMap>();
            this.AccountFolderMaps = new List<AccountFolderMap>();
            this.Folder1 = new List<Folder>();
            this.FolderVersions = new List<FolderVersion>();
            this.FolderLock = new List<FolderLock>();
            this.WorkFlowStateUserMaps = new List<WorkFlowStateUserMap>();
            this.WorkFlowStateFolderVersionMaps = new List<WorkFlowStateFolderVersionMap>();
            this.EmailLogs = new List<EmailLog>();
            this.FormInstanceActivityLogs = new List<FormInstanceActivityLog>();
        }

        public int FolderID { get; set; }
        public bool IsPortfolio { get; set; }
        public string Name { get; set; }
        public int MarketSegmentID { get; set; }
        public string PrimaryContent { get; set; }
        public Nullable<int> PrimaryContentID { get; set; }
        public int TenantID { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<int> ParentFolderId { get; set; }
        public Nullable<int> FormDesignGroupId { get; set; }
        public virtual ICollection<AccountProductMap> AccountProductMaps { get; set; }
        public virtual FormDesignGroup FormDesignGroup { get; set; }
        public virtual ICollection<Folder> Folder1 { get; set; }
        public virtual Folder Folder2 { get; set; }
        public virtual MarketSegment MarketSegment { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<FolderVersion> FolderVersions { get; set; }
        public virtual ICollection<AccountFolderMap> AccountFolderMaps { get; set; }
        public virtual ICollection<FolderLock> FolderLock { get; set; }
        public virtual ICollection<WorkFlowStateUserMap> WorkFlowStateUserMaps { get; set; }
        public virtual ICollection<WorkFlowStateFolderVersionMap> WorkFlowStateFolderVersionMaps { get; set; }
        public virtual ICollection<EmailLog> EmailLogs { get; set; }
        public virtual ICollection<FormInstanceActivityLog> FormInstanceActivityLogs { get; set; }
        public virtual ICollection<IASFolderMap> IASFolderMaps { get; set; }
        public Nullable<int> MasterListFormDesignID { get; set; }
        public bool IsFoundation{ get; set; }
    }
}
