using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class ServiceGroupModel : Entity
    {
        public string PDPD_ID { get; set; }
        public string ServiceGroupHeaderDescription { get; set; }
        public string ServiceGroupName { get; set; }

        public ServiceGroupModel Clone()
        {
            return this.MemberwiseClone() as ServiceGroupModel;
        }
    }
}
