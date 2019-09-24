using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class ServiceDesignVersionServiceDefinitionMap : Entity
    {
        public ServiceDesignVersionServiceDefinitionMap()
        {

        }

        public int ServiceDesignVersionServiceDefinitionMapID { get; set; }
        public int ServiceDesignVersionID { get; set; }
        public int ServiceDefinitionID { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? CancelDate { get; set; }

        public virtual ServiceDesignVersion ServiceDesignVersion { get; set; }
        public virtual ServiceDefinition ServiceDefinition { get; set; }
    }
}
