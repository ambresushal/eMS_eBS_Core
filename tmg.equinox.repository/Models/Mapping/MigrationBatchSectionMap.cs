using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class MigrationBatchSectionMap : EntityTypeConfiguration<MigrationBatchSection>
    {
        public MigrationBatchSectionMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Table & Column Mappings
            this.ToTable("MigrationBatchSection", "ODM");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.BatchId).HasColumnName("BatchId");
            this.Property(t => t.SectionGeneratedName).HasColumnName("SectionGeneratedName");
            this.Property(t => t.ViewType).HasColumnName("ViewType");

            // Relationships
            this.HasRequired(t => t.MigrationBatch)
                .WithMany(t => t.MigrationBatchSection)
                .HasForeignKey(d => d.BatchId);

        }
    }
}
