using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace tmg.equinox.domain.entities.Models
{
    public partial class SyncDocumentMacro : Entity
    {
        public SyncDocumentMacro()
        {
            this.SyncGroupLogs = new List<SyncGroupLog>();
        }

        public int MacroID { get; set; }
        public string MacroJSON { get; set; }
        public string MacroName { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool isLocked { get; set; }
        public bool isPublic{ get; set; }
        public string Notes { get; set; }
        public virtual ICollection<SyncGroupLog> SyncGroupLogs { get; set; }
    }
}
