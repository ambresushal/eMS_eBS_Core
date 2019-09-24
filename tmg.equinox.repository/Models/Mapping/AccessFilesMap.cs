using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class AccessFilesMap : EntityTypeConfiguration<AccessFiles>
    {
        public AccessFilesMap()
        {
            // Primary Key
            this.HasKey(t => t.FileID);

            // Table & Column Mappings
            this.ToTable("AccessFiles", "ODM");
            this.Property(t => t.FileID).HasColumnName("FileID");
            this.Property(t => t.BatchId).HasColumnName("BatchId");
            this.Property(t => t.FileName).HasColumnName("FileName");
            this.Property(t => t.OriginalFileName).HasColumnName("OriginalFileName");

            // Relationships
            this.HasRequired(t => t.MigrationBatch)
                .WithMany(t => t.AccessFilesMap)
                .HasForeignKey(d => d.BatchId);
        }
    }
}
