using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{

    public partial class WorkFlowCategoryMapping : Entity
    {
        public int TenantID { get; set; }
        public int WorkFlowVersionID { get; set; }
        public int FolderVersionCategoryID { get; set; }
        public int AccountType { get; set; }
        public int WorkFlowType { get; set; }
        public Nullable<DateTime> EffectiveDate { get; set; }
        public DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }
        public bool? IsFinalized { get; set; }

        public virtual FolderVersionCategory FolderVersionCategory { get; set; }
        public virtual Tenant Tenant { get; set; }

        public virtual ICollection<WorkFlowVersionState> WorkFlowVersionStates { get; set; }

    }
}
