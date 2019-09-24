using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
   public class PBPUserActionMap: EntityTypeConfiguration<PBPUserAction>
    {
        public PBPUserActionMap()
        {
            // Primary Key
            this.HasKey(t => t.PBPUserActionID);

            // Table & Column Mappings
            this.ToTable("PBPUserAction", "Setup");
            this.Property(t => t.PBPUserActionID).HasColumnName("PBPUserActionID");
            this.Property(t => t.UserAction).HasColumnName("UserAction");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
