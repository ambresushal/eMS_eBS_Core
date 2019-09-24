using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class TemplateUIMap : Entity
    {
        public int TemplateUIMapID { get; set; }
        public Nullable<int> TemplateID { get; set; }
        public Nullable<int> UIElementID { get; set; }
        public bool IncludeInPDF { get; set; }       
        public Nullable<int> TenantID { get; set; }        
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }


        public virtual Template Template { get; set; }
        public virtual UIElement UIElement { get; set; }
        public virtual Tenant Tenant { get; set; }

    }
}
