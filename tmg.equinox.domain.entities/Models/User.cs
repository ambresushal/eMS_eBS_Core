using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class User : Entity
    {
        public User()
        {
            this.Folders = new List<Folder>();
            this.FolderVersionWorkFlowStates = new List<FolderVersionWorkFlowState>();
            this.AutoSaveSettings = new List<AutoSaveSettings>();
            this.FolderLock = new List<FolderLock>();
            this.EmailLogs = new List<EmailLog>();
            this.ApplicableTeamUserMaps = new List<ApplicableTeamUserMap>();
            this.WorkFlowStateUserMaps = new List<WorkFlowStateUserMap>();
            this.UserRoleAssocMap = new List<UserRoleAssoc>();
            this.UserClaimMap = new List<UserClaim>();
            this.InterstedFolderVersions = new List<InterestedFolderVersion>();
            //  this.UserRole = new List<UserRole>();
        }

        public int UserID { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Nullable<bool> EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public bool IsVisible { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Nullable<bool> PhoneNumberConfirmed { get; set; }
        public Nullable<bool> TwoFactorEnabled { get; set; }
        public Nullable<System.DateTime> LockoutEndDateUtc { get; set; }
        public Nullable<bool> LockoutEnabled { get; set; }
        public Nullable<int> AccessFailedCount { get; set; }
        public Nullable<int> TenantID { get; set; }
        public bool ChangeInitialPassword { get; set; }
        public virtual ICollection<Folder> Folders { get; set; }
        public virtual ICollection<FolderVersionWorkFlowState> FolderVersionWorkFlowStates { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual ICollection<AutoSaveSettings> AutoSaveSettings { get; set; }
        public virtual ICollection<FolderLock> FolderLock { get; set; }
        public virtual ICollection<EmailLog> EmailLogs { get; set; }
        public virtual ICollection<ApplicableTeamUserMap> ApplicableTeamUserMaps { get; set; }
        public virtual ICollection<WorkFlowStateUserMap> WorkFlowStateUserMaps { get; set; }
        public virtual ICollection<UserRole> UserRole { get; set; }
        public virtual ICollection<UserRoleAssoc> UserRoleAssocMap { get; set; }
        public virtual ICollection<UserClaim> UserClaimMap { get; set; }
        public virtual ICollection<InterestedFolderVersion> InterstedFolderVersions { get; set; }

    }
}
