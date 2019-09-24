using System;

namespace tmg.equinox.applicationservices.viewmodels.DashBoard
{
    public class WatchListViewModel : ViewModelBase
    {
        public int FolderId { get; set; }
        public int FolderVersionId { get; set; }
        public string  Folder { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
        public string Owner { get; set; }
        public string Comments { get; set; }
        public int TenantID { get; set; }
        public int ApprovalStatusID { get; set; }
        public string FolderVersionNumber { get; set; }
        public string Account { get; set; }
        public bool IsActive { get; set; }
        public bool MarkInterested { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int AssignedUserCount { get; set; }
    }
}

