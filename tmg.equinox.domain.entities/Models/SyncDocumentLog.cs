using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class SyncDocumentLog : Entity
    {
        public SyncDocumentLog()
        {
        }

        public int SyncDocumentLogID { get; set; }
        public int SyncGroupLogID { get; set; }
        public int SourceDocumentID { get; set; }
        public int TargetDocumentID { get; set; }
        public int TargetDesignVersionID { get; set; }
        public int TargetDesignID { get; set; }
        public bool IsSyncAllowed { get; set; }
        public string TargetFormDataBU  { get; set; }
        public bool SyncCompleted{ get; set; }
        public string SyncJSONData { get; set; }
        public int FolderID { get; set; }
        public int FolderVersionID { get; set; }
        public System.DateTime LastUpdatedDate { get; set; }
        public string Notes { get; set; }
        public virtual SyncGroupLog SyncGroupLog { get; set; }
    }
}
