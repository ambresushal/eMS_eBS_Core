using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class Consortium : Entity
    {
        public Consortium()
        {
            this.FolderVersions = new List<FolderVersion>();
        }
        public int? ConsortiumID { get; set; }
        public string ConsortiumName { get; set; }
        public bool IsActive { get; set; }
        public int? TenantID { get; set; }
        public DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }
        public virtual ICollection<FolderVersion> FolderVersions { get; set; }
    }
}
