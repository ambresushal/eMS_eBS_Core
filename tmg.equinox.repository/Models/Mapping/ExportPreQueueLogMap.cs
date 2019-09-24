using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ExportPreQueueLogMap : EntityTypeConfiguration<ExportPreQueueLog>
    {
        public ExportPreQueueLogMap()
        {
            // Primary Key
            this.HasKey(t => t.ExportPreQueueLog1Up);

            // Table & Column Mappings
            this.ToTable("ExportPreQueueLog", "Setup");
            this.Property(t => t.ExportPreQueueLog1Up).HasColumnName("ExportPreQueueLog1Up");
            this.Property(t => t.ExportPreQueueId).HasColumnName("ExportPreQueueId");
            this.Property(t => t.FromInstanceId).HasColumnName("FromInstanceId");
            this.Property(t => t.ViewType).HasColumnName("ViewType");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.ErrorLog).HasColumnName("ErrorLog");
            // Relationships
        }
    }
}
