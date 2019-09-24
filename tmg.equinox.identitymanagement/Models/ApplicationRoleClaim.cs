using System;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace tmg.equinox.identitymanagement.Models
{
    public class ApplicationRoleClaim : IdentityUserClaim<int>
    {
        public ApplicationRoleClaim()
        {
  
        }
       
        public int RoleID { get; set; }

        public string ResourceType { get; set; }

        public Nullable<int> ResourceID { get; set; }
        
        public virtual ApplicationUserRole UserRole { get; set; }
      
        public virtual ApplicationUser User { internal get; set; }

        public virtual ApplicationRole Role { internal get; set; }     
    }
}