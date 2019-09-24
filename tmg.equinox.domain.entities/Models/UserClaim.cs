using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class UserClaim:Entity
    {
        public UserClaim() { 
        
        }
        public int ClaimID { get; set; }
        public int ResourceID { get; set; }
        public string Resource { get; set; }
        public string Action { get; set; }
        public string ResourceType { get; set; }
        public User user { get; set; }
        public UserRole userRole { get; set; }
        public int RoleID { get; set; }
        public int UserID { get; set; }
        public bool IsActive { get; set; }


    }
}
