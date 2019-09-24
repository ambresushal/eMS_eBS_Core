using System;
using System.Collections.Generic;
using tmg.equinox.applicationservices.viewmodels.Settings;

namespace tmg.equinox.applicationservices.viewmodels.FolderVersion
{
    public class FolderVersionViewModel : ViewModelBase
    {
        public int FolderVersionId { get; set; }
        public int FolderId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public int? WFStateID { get; set; }
        public string WFStateName { get; set; }
        public string FolderVersionNumber { get; set; }
        public int VersionTypeID { get; set; }
        public bool IsActive { get; set; }
        public int TenantID { get; set; }
        public string FolderName { get; set; }
        public int? AccountId { get; set; }
        public bool IsPortfolio { get; set; }
        public bool IsEditable { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public DateTime BaseLineDate { get; set; }
        public string UserName { get; set; }
        public int UserID { get; set; }
        public string Comments { get; set; }
        public string FolderType { get; set; }
        public bool IsCopyRetro { get; set; }
        public int FolderVersionStateID { get; set; }
        public string VersionType { get; set; }
        public int FormDesignVersionType { get; set; }
        public string FormDesignDisplayText { get; set; }
        public string AutoSaveSettingsProperties { get; set; }
        public bool IsAutoSaveEnabled { get; set; }
        public int Duration { get; set; }
        public bool IsReleased { get; set; }
        public bool IsLocked { get; set; }
        public int? LockedBy { get; set; }
        public string LockedByUser { get; set; }
        public DateTime LockedDate { get; set; }
        public bool IsNewVersionLoaded { get; set; }
        public bool IsNewLoadedVersionIsMajorVersion { get; set; }
        public string CurrentUserName { get; set; }
        public int? CurrentUserId { get; set; }
        public int RoleId { get; set; }
        public int? ConsortiumID { get; set; }
        public int? CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string CatID { get; set; }
        public int? ReferenceProductFormInstanceID { get; set; }
        public string AccountName { get; set; }
        public string PrimaryContact { get; set; }
        public string urls { get; set; }
        public bool IsMasterList { get; set; }
        public string FoldeViewMode { get; set; }
        public int FormInstanceID { get; set; }
        public int DocId { get; set; }
        public bool Select { get; set;}
        //public bool IsLockEnable { get; set; }
        //public bool CurrentUserName { get; set; }
    }
    public class ActivityLogModel
    {
        public int ID { get; set; }
        public string SubSectionName { get; set; }
        public string ElementPath { get; set; }
        public string Field { get; set; }
        public string RowNum { get; set; }
        public string Description { get; set; }
        public string UpdatedBy { get; set; }
        public string FolderVersionName { get; set; }
        public System.DateTime UpdatedLast { get; set; }
        public bool IsNewRecord { get; set; }
        public int FormInstanceID { get; set; }
    }

    public class ReferenceDocumentViewModel
    {
        public string AccountName { get; set; }
        public string FolderName { get; set; }
        public string FolderVersionNumber { get; set; }
        public string DocumentName { get; set; }
        public System.DateTime FolderVersionEffectiveDate { get; set; }
        public int? ConsortiumID { get; set; }
    }
    public class FolderVersionAPIViewModel
    {
        public int FolderID { get; set; }
        public int DocumentID { get; set; }
        public string DocumentName { get; set; }
        public string FolderVersion { get; set; }
        public string EffectiveDate { get; set; }
        public string Status { get; set; }
        public IList<Documents> Document { get; set; }
    }
    public class FolderVersions
    {
        public int FolderVersionID { get; set; }
        public string FolderName { get; set; }
        public string FolderVersionNumber { get; set; }
        public string EffectiveDate { get; set; }
        public string Status { get; set; }
        public IList<Documents> Document { get; set; }
    }
    public class Documents
    {
        public string DocumentID { get; set; }
        public string DocumentName { get; set; }
        public string DesignTemplate { get; set; }
        public string DesignTemplateVersion { get; set; }
        public IList<url> Links { get; set; }
    }
    public class url
    {
        public string rowTemplate { get; set; }
    }
    public class FolderVersionState
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
}
