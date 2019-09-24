using System;

namespace tmg.equinox.applicationservices.viewmodels.DashBoard
{
    public class WorkQueueViewModel : ViewModelBase
    {
        public int FolderId { get; set; }
        public int FolderVersionId { get; set; }
        public string Folder { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
        public string Name { get; set; }
        public int TenantID { get; set; }
        public string FolderVersionNumber { get; set; }
        public string Account { get; set; }
        public bool IsActive { get; set; }
    }
}
