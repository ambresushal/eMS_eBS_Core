using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class WorkFlowVersionStatesAccess : Entity
    {
        public int WorkFlowVersionStatesAccessID { get; set; }
        public int RoleID { get; set; }
        public int WorkFlowVersionStateID { get; set; }
        //public bool EditPermission { get; set; }
        //public bool IsOwner { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }

        public virtual WorkFlowVersionState WorkFlowVersionStates { get; set; }
        public virtual UserRole UserRole { get; set; }
    }
}
