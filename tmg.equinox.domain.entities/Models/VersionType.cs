using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class VersionType : Entity
    {
        public VersionType()
        {
            this.FolderVersions = new List<FolderVersion>();
        }

        public int VersionTypeID { get; set; }
        public int TenantID { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string VersionType1 { get; set; }
        public virtual ICollection<FolderVersion> FolderVersions { get; set; }
        public virtual Tenant Tenant { get; set; }
    }
}
