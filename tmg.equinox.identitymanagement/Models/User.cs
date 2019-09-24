using Microsoft.AspNet.Identity.EntityFramework;
using tmg.equinox.identitymanagement.Models.Mapping;

namespace tmg.equinox.identitymanagement.Models
{
    public class ApplicationUser : IdentityUser<int, ApplicationUserLogin, ApplicationUserUserRole, ApplicationUserClaim>
    {
        public ApplicationUser()
        {
            //this.Logins = new List<ApplicationUserLogin>();
            //this.UserRoles = new List<ApplicationUserRole>();
            //Claims = new List<ApplicationUserClaim>();
        }

        //[Key]
        /*public override int Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                base.Id = value;
            }
        }*/

        public string AuthorizationId { get; set; }

        /*
                [Key]
                public virtual int UserID { get; set; }*/
        /*public virtual ICollection<ApplicationUserLogin> Logins
        {
            get;
            set;
        }*/

        //public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
        //public virtual ICollection<ApplicationUserClaim> Claims { get; set; }
    }
}
