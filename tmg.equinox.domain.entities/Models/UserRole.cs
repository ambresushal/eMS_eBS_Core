using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class UserRole:Entity
    {
        public UserRole()
        {
            //this.UserClaims = new List<UserClaim>();
            this.Users = new List<User>();
            this.UserRoleAssocMap = new List<UserRoleAssoc>();
            this.UserClaimMap = new List<UserClaim>();
        }

        public int RoleID { get; set; }
        public string Name { get; set; }
       // public virtual ICollection<UserClaim> UserClaims { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<UserRoleAssoc> UserRoleAssocMap { get; set; }
        public virtual ICollection<UserClaim> UserClaimMap { get; set; }
        public virtual ICollection<TemplateReportRoleAccessPermission> TemplateReportRoleAccessPermissions { get; set; }
        public virtual ICollection<WorkFlowVersionStatesAccess> WorkFlowVersionStatesAccess { get; set; }
    }
}
