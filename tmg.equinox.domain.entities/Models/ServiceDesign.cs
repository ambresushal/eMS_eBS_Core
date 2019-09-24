using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class ServiceDesign : Entity
    {
        public ServiceDesign()
        {
            this.ServiceDesignVersions = new List<ServiceDesignVersion>();
            this.ServiceParameters = new List<ServiceParameter>();
        }

        public int ServiceDesignID { get; set; }
        public string ServiceName { get; set; }
        public string ServiceMethodName { get; set; }
        public bool IsActive { get; set; }
        public bool DoesReturnList { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int TenantID { get; set; }
        public bool IsReturnJSON { get; set; }

        public virtual ICollection<ServiceDesignVersion> ServiceDesignVersions { get; set; }
        public virtual ICollection<ServiceDefinition> ServiceDefinitions { get; set; }
        public virtual ICollection<ServiceParameter> ServiceParameters { get; set; }
        public virtual Tenant Tenant { get; set; }
    }
}
