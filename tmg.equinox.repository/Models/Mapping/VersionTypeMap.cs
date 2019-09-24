using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class VersionTypeMap : EntityTypeConfiguration<VersionType>
    {
        public VersionTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.VersionTypeID);

            // Properties
            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.VersionType1)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("VersionType", "Fldr");
            this.Property(t => t.VersionTypeID).HasColumnName("VersionTypeID");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.VersionType1).HasColumnName("VersionType");

            // Relationships
            this.HasRequired(t => t.Tenant)
                .WithMany(t => t.VersionTypes)
                .HasForeignKey(d => d.TenantID);

        }
    }
}
