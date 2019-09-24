using System;
using System.Collections.Generic;

namespace tmg.equinox.applicationservices.viewmodels.comparesync
{
    public class SyncGroupLogViewModel : ViewModelBase
    {
        public int SyncGroupLogID { get; set; }
        public string SyncBy { get; set; }
        public System.DateTime SyncDate { get; set; }
        public int MacroID { get; set; }
        public int SourceDocumentID { get; set; }
        public int FolderID { get; set; }
        public int FolderVersionID { get; set; }
        public string FolderVersionNumber { get; set; }
        public System.DateTime LastUpdatedDate { get; set; }
        public string Notes { get; set; }

    }
}
