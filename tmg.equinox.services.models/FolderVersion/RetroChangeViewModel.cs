using System;

namespace tmg.equinox.applicationservices.viewmodels.FolderVersion
{
    public class RetroChangeViewModel : ViewModelBase
    {
        public int FolderVersionId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsCopyRetro { get; set; }
        public int FolderId { get; set; }
        public int TenantId { get; set; }
        public string FolderVersionNumber { get; set; }
        public double VersionNumber { get; set; }
        public string Comments { get; set; }
    }
}
