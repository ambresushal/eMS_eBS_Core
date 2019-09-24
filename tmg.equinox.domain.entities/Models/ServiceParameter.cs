using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class ServiceParameter : Entity
    {
        public ServiceParameter()
        {

        }

        public int ServiceParameterID { get; set; }
        public string Name { get; set; }
        public string GeneratedName { get; set; }
        public int DataTypeID { get; set; }
        public bool IsRequired { get; set; }
        public int ServiceDesignID { get; set; }
        public int ServiceDesignVersionID { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public int TenantID { get; set; }
        public int? UIElementID { get; set; }
        public string UIElementFullPath { get; set; }

        public virtual ApplicationDataType ApplicationDataType { get; set; }
        public virtual ServiceDesign ServiceDesign { get; set; }
        public virtual ServiceDesignVersion ServiceDesignVersion { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual UIElement UIElement { get; set; }
    }
}
