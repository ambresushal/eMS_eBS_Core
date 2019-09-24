using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class MigrationBatchsMap : EntityTypeConfiguration<MigrationBatchs>
    {
        public MigrationBatchsMap()
        {
            // Primary Key
            this.HasKey(t => t.BatchId);

            // Table & Column Mappings
            this.ToTable("MigrationBatchs", "ODM");
            this.Property(t => t.BatchId).HasColumnName("BatchId");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.QueuedDate).HasColumnName("QueuedDate");
            this.Property(t => t.QueuedUser).HasColumnName("QueuedUser");
            this.Property(t => t.ExecutedDate).HasColumnName("ExecutedDate");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
