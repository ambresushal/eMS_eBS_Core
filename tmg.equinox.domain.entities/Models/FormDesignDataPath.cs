using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class FormDesignDataPath : Entity
    {
        public FormDesignDataPath() { }
        public int FormDesignDataPathID { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public Nullable<int> FormDesignID { get; set; }
        public Nullable<int> FormDesignVersionID { get; set; }
        public int TenantID { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual FormDesign FormDesign { get; set; }
        public virtual FormDesignVersion FormDesignVersion { get; set; }

    }
}
