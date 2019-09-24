using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class ServiceDesignVersion : Entity
    {
        public ServiceDesignVersion()
        {
            this.ServiceDesignVersionServiceDefinitionMaps = new List<ServiceDesignVersionServiceDefinitionMap>();
            this.ServiceParameters = new List<ServiceParameter>();
        }

        public int ServiceDesignVersionID { get; set; }
        public int ServiceDesignID { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }
        public string VersionNumber { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsFinalized { get; set; }
        public bool IsActive { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int TenantID { get; set; }
        public string ServiceDesignData { get; set; }

        public virtual ServiceDesign ServiceDetail { get; set; }
        public virtual FormDesign FormDesign { get; set; }
        public virtual FormDesignVersion FormDesignVersion { get; set; }
        public virtual ICollection<ServiceDesignVersionServiceDefinitionMap> ServiceDesignVersionServiceDefinitionMaps { get; set; }
        public virtual ICollection<ServiceParameter> ServiceParameters { get; set; }
        public virtual Tenant Tenant { get; set; }
    }
}
