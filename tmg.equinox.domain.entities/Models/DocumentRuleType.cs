using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class DocumentRuleType : Entity
    {
        public int DocumentRuleTypeID { get; set; }
        public string DocumentRuleTypeCode { get; set; }
        public string DisplayText { get; set; }
        public string Description { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<DocumentRule> DocumentRule { get; set; }
    }
}
