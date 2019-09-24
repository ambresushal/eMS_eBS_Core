using System;
using System.Collections.Generic;

namespace tmg.equinox.applicationservices.viewmodels.comparesync
{
    public class DocumentSyncResultViewModel : ViewModelBase
    {
        public int FolderVersionID { get; set; }
        public string FolderName { get; set; }
        public string VersionNumber { get; set; }
        public string AccountName { get; set; }
        public List<DocumentSyncResultRowViewModel> FormInstances { get; set; }
        public string SyncStatus { get; set; }
        public bool IsFolderLock { get; set; }
    }
    public class DocumentSyncResultRowViewModel : ViewModelBase
    {
        public int FormInstanceID { get; set; }

        public string FormInstanceName { get; set; }

        public string SyncStatus { get; set; }
    }
}
