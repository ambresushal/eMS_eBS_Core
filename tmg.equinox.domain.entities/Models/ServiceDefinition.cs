using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class ServiceDefinition : Entity
    {
        public ServiceDefinition()
        {
            this.ServiceDesignVersionServiceDefinitionMaps = new List<ServiceDesignVersionServiceDefinitionMap>();
            this.ServiceDefinitions = new List<ServiceDefinition>();
        }

        public int ServiceDefinitionID { get; set; }
        public string UIElementFullPath { get; set; }
        public int UIElementDataTypeID { get; set; }
        public int UIElementTypeID { get; set; }
        public int UIElementID { get; set; }
        public string DisplayName { get; set; }
        public int? ParentServiceDefinitionID { get; set; }
        public int ServiceDesignID { get; set; }
        //public bool IsActive { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int TenantID { get; set; }
        public int Sequence { get; set; }
        public bool IsKey { get; set; }
        public bool IsRequired { get; set; }
        public bool IsPartOfDataSource { get; set; }

        public virtual UIElement UIElement { get; set; }
        public virtual ApplicationDataType ApplicationDataType { get; set; }
        public virtual UIElementType UIElementType { get; set; }
        public virtual ServiceDefinition ParentServiceDefinition { get; set; }
        public virtual ServiceDesign ServiceDesign { get; set; }

        public virtual ICollection<ServiceDefinition> ServiceDefinitions { get; set; }
        public virtual ICollection<ServiceDesignVersionServiceDefinitionMap> ServiceDesignVersionServiceDefinitionMaps { get; set; }
        public virtual Tenant Tenant { get; set; }
    }
}
