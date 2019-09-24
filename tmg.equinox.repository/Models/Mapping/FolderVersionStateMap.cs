using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FolderVersionStateMap : EntityTypeConfiguration<FolderVersionState>
    {
        public FolderVersionStateMap()
        {
            // Primary Key
            this.HasKey(t => t.FolderVersionStateID);

            // Properties
            this.Property(t => t.FolderVersionStateName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("FolderVersionState", "Fldr");
            this.Property(t => t.FolderVersionStateID).HasColumnName("FolderVersionStateID");
            this.Property(t => t.FolderVersionStateName).HasColumnName("FolderVersionStateName");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
        }
    }
}
