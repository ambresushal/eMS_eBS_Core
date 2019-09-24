using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class PluginProcessorErrorMap : EntityTypeConfiguration<PluginProcessorError>
    {
        public PluginProcessorErrorMap()
        {
            // Primary Key
            this.HasKey(t => t.ErrorId);

            // Properties
            this.Property(t => t.BatchId)
                .HasMaxLength(50);

            this.Property(t => t.ErrorDescription)
                .HasMaxLength(500);

            this.Property(t => t.CreatedBy)
                .HasMaxLength(50);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("PluginProcessorError", "setup");
            this.Property(t => t.ErrorId).HasColumnName("ErrorId");
            this.Property(t => t.ProcessQueueId).HasColumnName("ProcessQueueId");
            this.Property(t => t.BatchId).HasColumnName("BatchId");
            this.Property(t => t.ProductId).HasColumnName("ProductId");
            this.Property(t => t.ErrorDescription).HasColumnName("ErrorDescription");
            this.Property(t => t.ErrorLine).HasColumnName("ErrorLine");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
        }
    }
}
