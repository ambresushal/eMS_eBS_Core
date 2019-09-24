using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class PluginVersionProcessorMap : EntityTypeConfiguration<PluginVersionProcessor>
    {
        public PluginVersionProcessorMap()
        {
            // Primary Key
            this.HasKey(t => t.PluginVersionProcessorId);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(200);

            this.Property(t => t.OutPutFormat)
                .HasMaxLength(200);

            this.Property(t => t.CreatedBy)
                .HasMaxLength(50);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("PluginVersionProcessor", "setup");
            this.Property(t => t.PluginVersionProcessorId).HasColumnName("PluginVersionProcessorId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.OutPutFormat).HasColumnName("OutPutFormat");
            this.Property(t => t.PluginVersionId).HasColumnName("PluginVersionId");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
        }
    }
}
