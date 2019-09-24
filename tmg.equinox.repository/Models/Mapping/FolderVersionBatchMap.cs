using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FolderVersionBatchMap : EntityTypeConfiguration<FolderVersionBatch>
    {
        public FolderVersionBatchMap()
        {
            // Primary Key
            this.HasKey(t => t.FolderVersionBatchID);

            // Properties
            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("FolderVersionBatch", "Fldr");
            this.Property(t => t.FolderVersionBatchID).HasColumnName("FolderVersionBatchID");
            this.Property(t => t.EffectiveDate).HasColumnName("EffectiveDate");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
        }
    }
}
