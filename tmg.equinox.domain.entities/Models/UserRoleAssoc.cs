using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class UserRoleAssoc :Entity
    {
        public UserRoleAssoc(){
    
    }

        public int UserId { get; set; }
        public int RoleId { get; set; }
        public virtual User User { get; set; }
        public virtual UserRole UserRole { get; set; }
        public bool IsActive { get; set; }

    }
}
