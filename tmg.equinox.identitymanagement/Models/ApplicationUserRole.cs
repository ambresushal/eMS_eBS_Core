using Microsoft.AspNet.Identity.EntityFramework;

namespace tmg.equinox.identitymanagement.Models
{
    public class ApplicationUserRole : IdentityUserRole<int>
    {        
        public virtual ApplicationUser User { internal get; set; }

        public virtual ApplicationRole Role { internal get; set; }                                                                                                                                                                                                                                 
    }
}