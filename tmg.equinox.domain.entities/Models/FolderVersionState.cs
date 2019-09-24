using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class FolderVersionState : Entity
    {
        public FolderVersionState()
        {
            this.FolderVersions = new List<FolderVersion>();
            this.EmailLogs = new List<EmailLog>();
        }

        public int FolderVersionStateID { get; set; }
        public string FolderVersionStateName { get; set; }
        public int TenantID { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public virtual ICollection<FolderVersion> FolderVersions { get; set; }
        public virtual ICollection<EmailLog> EmailLogs { get; set; }
    }
}
