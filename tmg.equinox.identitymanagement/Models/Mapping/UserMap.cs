using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.identitymanagement.Models.Mapping
{
    public class UserMap : EntityTypeConfiguration<ApplicationUser>
    {
        public UserMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.UserName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Email)
                .HasMaxLength(100);

            this.Property(t => t.PasswordHash)
                .HasMaxLength(100);

            this.Property(t => t.SecurityStamp)
                .HasMaxLength(100);

            this.Property(t => t.PhoneNumber)
                .HasMaxLength(25);

            // Table & Column Mappings
            this.ToTable("User", "Sec");
            this.Property(t => t.Id).HasColumnName("UserID");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.EmailConfirmed).HasColumnName("EmailConfirmed");
            this.Property(t => t.PasswordHash).HasColumnName("PasswordHash");
            this.Property(t => t.SecurityStamp).HasColumnName("SecurityStamp");
            this.Property(t => t.PhoneNumber).HasColumnName("PhoneNumber");
            this.Property(t => t.PhoneNumberConfirmed).HasColumnName("PhoneNumberConfirmed");
            this.Property(t => t.TwoFactorEnabled).HasColumnName("TwoFactorEnabled");
            this.Property(t => t.LockoutEndDateUtc).HasColumnName("LockoutEndDateUtc");
            this.Property(t => t.LockoutEnabled).HasColumnName("LockoutEnabled");
            this.Property(t => t.AccessFailedCount).HasColumnName("AccessFailedCount");
            this.Property(t => t.FirstName).HasColumnName("FirstName");
            this.Property(t => t.LastName).HasColumnName("LastName");


           // this.HasMany(u => u.Roles).WithRequired().HasForeignKey(ur =>ur.RoleId);



            //entity.HasMany(u => u.Roles).WithRequired().HasForeignKey(ur => ur.UserId);

           /* HasMany(p => p.Logins)
            .WithRequired(t => t.User)
            .HasForeignKey(n => n.UserId);*/
            //this.Property(t => t.TenantID).HasColumnName("TenantID");

            //Relationships
            /* this.HasMany(t => t.UserRoles)
                .WithMany(t => t.Users)
                .Map(m =>
                {
                    //m.ToTable("UserUserRole", "Sec");
                    m.MapLeftKey("UserID");
                    m.MapRightKey("RoleID");
                });*/

            //this.HasOptional(t => t.Tenant)
            //    .WithMany(t => t.Users)
            //    .HasForeignKey(d => d.TenantID);

        }
    }
}
