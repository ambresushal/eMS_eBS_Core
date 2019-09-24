using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class UserClaimMap : EntityTypeConfiguration<UserClaim>
    {
        public UserClaimMap()
        {
            // Primary Key
            this.HasKey(t => t.ClaimID);
            this.Property(t => t.IsActive)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("UserClaim", "Sec");
            this.Property(t => t.ResourceID).HasColumnName("ResourceID");
            this.Property(t => t.Resource).HasColumnName("Resource");
            this.Property(t => t.Action).HasColumnName("Action");
            this.Property(t => t.ResourceType).HasColumnName("ResourceType");
            this.Property(t => t.IsActive).HasColumnName("IsActive");

            this.HasRequired(t => t.userRole)
               .WithMany(t => t.UserClaimMap)
               .HasForeignKey(d => d.RoleID);

            this.HasRequired(t => t.user)
               .WithMany(t => t.UserClaimMap)
               .HasForeignKey(d => d.UserID);
        }
    }
}
