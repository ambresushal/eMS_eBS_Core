using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class RuleTargetTypeMap : EntityTypeConfiguration<RuleTargetType>
    {
        public RuleTargetTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.RuleTargetTypeID);

            // Properties
            this.Property(t => t.RuleTargetTypeCode)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.AddedBy)
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.DisplayText)
                .HasMaxLength(20);

            this.Property(t => t.Description)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("RuleTargetType", "UI");
            this.Property(t => t.RuleTargetTypeID).HasColumnName("RuleTargetTypeID");
            this.Property(t => t.RuleTargetTypeCode).HasColumnName("RuleTargetTypeCode");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.DisplayText).HasColumnName("DisplayText");
            this.Property(t => t.Description).HasColumnName("Description");
        }
    }
}
