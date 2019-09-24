using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.identitymanagement.Models.Mapping
{
    public class RoleClaimMap : EntityTypeConfiguration<ApplicationRoleClaim>
    {
        public RoleClaimMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("UserClaim", "Sec");
            this.Property(t => t.Id).HasColumnName("ClaimID");
            this.Property(t => t.ClaimType).HasColumnName("Resource");
            this.Property(t => t.ClaimValue).HasColumnName("Action");
            this.Property(t => t.RoleID).HasColumnName("RoleID");
            this.Property(t => t.ResourceType).HasColumnName("ResourceType");
            this.Property(t => t.ResourceID).HasColumnName("ResourceID");
            HasRequired(t => t.Role).WithMany(t => t.RoleClaims).HasForeignKey(t => t.RoleID);
        }
    }
}