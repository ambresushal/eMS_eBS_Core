using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class FolderVersionBatch : Entity
    {
        public FolderVersionBatch()
        {
            this.FolderVersions = new List<FolderVersion>();
        }

        public int FolderVersionBatchID { get; set; }
        public System.DateTime EffectiveDate { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public virtual ICollection<FolderVersion> FolderVersions { get; set; }
    }
}
