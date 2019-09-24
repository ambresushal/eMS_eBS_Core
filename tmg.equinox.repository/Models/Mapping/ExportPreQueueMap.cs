using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ExportPreQueueMap : EntityTypeConfiguration<ExportPreQueue>
    {
        public ExportPreQueueMap()
        {
            // Primary Key
            this.HasKey(t => t.ExportPreQueue1Up);

            // Table & Column Mappings
            this.ToTable("ExportPreQueue", "Setup");
            this.Property(t => t.ExportPreQueue1Up).HasColumnName("ExportPreQueue1Up");
            this.Property(t => t.PBPExportId).HasColumnName("PBPExportId");
            this.Property(t => t.PBPDatabase1Up).HasColumnName("PBPDatabase1Up");
            this.Property(t => t.PreQueueStatus).HasColumnName("PreQueueStatus");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.CurrentUserId).HasColumnName("CurrentUserId");

            // Relationships
        }
    }
}
