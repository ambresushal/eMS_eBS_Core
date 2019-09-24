using System;
using Microsoft.AspNet.Identity.EntityFramework;

namespace tmg.equinox.identitymanagement.Models
{
    public class ApplicationUserClaim : IdentityUserClaim<int>
    {
        public int ClaimID { get; set; }

        public int RoleID { get; set; }        
      
        public virtual ApplicationUser User {internal get; set; }
    }
}