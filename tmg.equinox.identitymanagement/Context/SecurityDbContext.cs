using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using tmg.equinox.identitymanagement.Models;
using tmg.equinox.identitymanagement.Models.Mapping;

namespace tmg.equinox.identitymanagement
{
    //public class SecurityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int, ApplicationUserLogin, ApplicationUserRole, Claim>
    public class SecurityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int, ApplicationUserLogin, ApplicationUserRole, ApplicationRoleClaim>
    {
        #region Private Memebers
        #endregion Private Members

        #region Public Properties

        public IDbSet<ApplicationRoleClaim> RoleClaim { get; set; }

        #endregion Public Properties

        #region Constructor

        static SecurityDbContext()
        {
            Database.SetInitializer<SecurityDbContext>(null);
        }

        public SecurityDbContext()
            : base("name=UIFrameworkContext")
        {
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = false;
        }

        #endregion Constructor

        #region Public\Protected Methods

        //for OWIN
        public static SecurityDbContext Create()
        {
            return new SecurityDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new UserLoginMap());
            modelBuilder.Configurations.Add(new UserRoleMap());
            modelBuilder.Configurations.Add(new UserUserRoleMap());
            modelBuilder.Configurations.Add(new RoleClaimMap());

            //modelBuilder.Entity<ApplicationUser>().HasKey(t => t.Id);
            /*modelBuilder.Entity<ApplicationUser>().Ignore(t => t.AuthorizationId);  */
            //modelBuilder.Entity<ApplicationRole>().Ignore(t => t.Users);

        }

        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods
    }
}
