using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace tmg.equinox.identitymanagement.Models
{
    //public class ApplicationUser : IdentityUser<int, ApplicationUserLogin, ApplicationUserRole,Claim>
    public class ApplicationUser : IdentityUser<int, ApplicationUserLogin, ApplicationUserRole, ApplicationRoleClaim>        
    {        
        /*public string AuthorizationId { get; set; }*/

        public virtual int TenantID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
       
    }
}