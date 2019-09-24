using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class PluginVersionProcessQueueCommonMap : EntityTypeConfiguration<PluginVersionProcessQueueCommon>
    {
        public PluginVersionProcessQueueCommonMap()
        {
            // Primary Key
            this.HasKey(t => t.ProcessQueueId);

            // Properties
            //this.Property(t => t.Status)
            //    .HasMaxLength(200);

            // Properties
            this.Property(t => t.Product)
                .HasMaxLength(50);

            this.Property(t => t.BatchId)
                .HasMaxLength(50);

            this.Property(t => t.CreatedBy)
                .HasMaxLength(50);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(50);

            this.Property(t => t.FolderVersionNumber)
               .HasMaxLength(200);

            this.Property(t => t.FormInstanceName)
              .HasMaxLength(200);

            this.Property(t => t.FolderName)
              .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("PluginVersionProcessQueue", "setup");
            this.Property(t => t.ProcessQueueId).HasColumnName("ProcessQueueId");
            this.Property(t => t.PluginVersionProcessorId).HasColumnName("PluginVersionProcessorId");
            this.Property(t => t.ProductId).HasColumnName("ProductId");
            this.Property(t => t.Product).HasColumnName("Product");
            this.Property(t => t.StartTime).HasColumnName("StartTime");
            this.Property(t => t.EndTime).HasColumnName("EndTime");
            this.Property(t => t.PluginVersionStatusId).HasColumnName("PluginVersionStatusId");
            this.Property(t => t.HasError).HasColumnName("HasError");
            this.Property(t => t.BatchId).HasColumnName("BatchId");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.FolderVersionNumber).HasColumnName("FolderVersionNumber");
            this.Property(t => t.FormInstanceName).HasColumnName("FormInstanceName");
            this.Property(t => t.FolderName).HasColumnName("FolderName");
        }
    }
}
