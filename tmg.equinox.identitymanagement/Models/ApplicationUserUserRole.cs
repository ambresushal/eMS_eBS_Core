using Microsoft.AspNet.Identity.EntityFramework;

namespace tmg.equinox.identitymanagement.Models
{
    public class ApplicationUserUserRole : IdentityUserRole<int>
    {        
        public virtual ApplicationUser User { internal get; set; }
    }
}