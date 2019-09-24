using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class Template : Entity
    {
        public int TemplateID { get; set; }
        public Nullable<int> FormDesignID { get; set; }
        public Nullable<int> FormDesignVersionID { get; set; }
        public Nullable<int> TenantID { get; set; }
        public string TemplateName { get; set; }
        public string Description { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual FormDesign FormDesign { get; set; }
        public virtual FormDesignVersion FormDesignVersion { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual ICollection<TemplateUIMap> TemplateUIMap { get; set; }
    }
}
