using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.identitymanagement.Models
{
    public class ApplicationUserStore : UserStore<ApplicationUser, ApplicationRole, int, ApplicationUserLogin, ApplicationUserRole, ApplicationRoleClaim>
    {
        public ApplicationUserStore(SecurityDbContext context)
                : base(context)
            {
            }
        
    }
}
