using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using tmg.equinox.identitymanagement.Models.Mapping;

namespace tmg.equinox.identitymanagement.Models
{
    public class ApplicationRole : IdentityRole<int, ApplicationUserRole>
    {
        public ApplicationRole():base()                                 
        {            
            this.RoleClaims = new List<ApplicationRoleClaim>();
            //this.Users = new List<ApplicationUser>();
            
        }        
          
        public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; }        
    }
}

