using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class UserRoleAssocMap : EntityTypeConfiguration<UserRoleAssoc>
    {
        public UserRoleAssocMap() {


            // Primary Key
            this.HasKey(t => new { t.UserId,t.RoleId});
           
            // Properties
            this.Property(t => t.UserId).IsRequired();
            this.Property(t => t.RoleId).IsRequired();
            this.Property(t=>t.IsActive).IsRequired();

            // Table & Column Mappings
            this.ToTable("UserRoleAssoc", "Sec");
            this.Property(t => t.UserId).HasColumnName("UserID");
            this.Property(t => t.RoleId).HasColumnName("RoleID");
            this.Property(t => t.IsActive).HasColumnName("IsActive");

            this.HasRequired(t => t.UserRole)
               .WithMany(t => t.UserRoleAssocMap)
               .HasForeignKey(d => d.RoleId);

            this.HasRequired(t => t.User)
               .WithMany(t => t.UserRoleAssocMap)
               .HasForeignKey(d => d.UserId);

        
        }
    }
}
