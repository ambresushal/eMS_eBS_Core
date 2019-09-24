using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class SyncGroupLog : Entity
    {
        public SyncGroupLog()
        {
            this.SyncDocumentLogs = new List<SyncDocumentLog>();
        }

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
        public virtual SyncDocumentMacro SyncDocumentMacro { get; set; }
        public virtual ICollection<SyncDocumentLog> SyncDocumentLogs { get; set; }
    }
}
