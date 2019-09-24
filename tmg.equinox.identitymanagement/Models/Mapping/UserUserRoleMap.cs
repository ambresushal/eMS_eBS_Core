using System.Data.Entity.ModelConfiguration;
using Microsoft.AspNet.Identity.EntityFramework;

namespace tmg.equinox.identitymanagement.Models.Mapping
{
    public class UserUserRoleMap : EntityTypeConfiguration<ApplicationUserRole>
    {
        public UserUserRoleMap()
        {
            // Primary Key
            this.HasKey(t => new { t.RoleId, t.UserId });

            // Table & Column Mappings
            this.ToTable("UserRoleAssoc", "Sec");
            this.Property(t => t.RoleId).HasColumnName("RoleID");
            this.Property(t => t.UserId).HasColumnName("UserID");
        }
    }
}