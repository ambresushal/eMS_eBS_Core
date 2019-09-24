using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class MigrationPlansMap : EntityTypeConfiguration<MigrationPlans>
    {
        public MigrationPlansMap()
        {
            // Primary Key
            this.HasKey(t => t.MigrationPlanID);

            // Table & Column Mappings
            this.ToTable("MigrationPlans", "ODM");
            this.Property(t => t.MigrationPlanID).HasColumnName("MigrationPlanID");
            this.Property(t => t.BatchId).HasColumnName("BatchId");
            this.Property(t => t.FileID).HasColumnName("FileID");
            this.Property(t => t.FolderId).HasColumnName("FolderId");
            this.Property(t => t.FolderVersionId).HasColumnName("FolderVersionId");
            this.Property(t => t.FormInstanceId).HasColumnName("FormInstanceId");
            this.Property(t => t.FormDesignVersionId).HasColumnName("FormDesignVersionId");
            this.Property(t => t.ViewType).HasColumnName("ViewType");
            this.Property(t => t.QID).HasColumnName("QID");

            // Relationships
            this.HasRequired(t => t.MigrationBatch)
                .WithMany(t => t.MigrationPlans)
                .HasForeignKey(d => d.BatchId);

            // Relationships
            this.HasRequired(t => t.AccessFiles)
                .WithMany(t => t.MigrationPlans)
                .HasForeignKey(d => d.FileID);
        }
    }
}
