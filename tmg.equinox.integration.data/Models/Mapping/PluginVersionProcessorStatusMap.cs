using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class PluginVersionProcessorStatusMap : EntityTypeConfiguration<PluginVersionProcessorStatus>
    {
        public PluginVersionProcessorStatusMap()
        {
            // Primary Key
            this.HasKey(t => t.PluginVersionStatusId);

            // Properties
            this.Property(t => t.Status)
                .HasMaxLength(200);

            this.Property(t => t.Description)
                .HasMaxLength(200);

            this.Property(t => t.CreatedBy)
                .HasMaxLength(50);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("PluginVersionProcessorStatus", "setup");
            this.Property(t => t.PluginVersionStatusId).HasColumnName("PluginVersionStatusId");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
        }
    }
}
