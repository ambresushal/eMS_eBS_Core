using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class MarketSegmentMap : EntityTypeConfiguration<MarketSegment>
    {
        public MarketSegmentMap()
        {
            // Primary Key
            this.HasKey(t => t.MarketSegmentID);

            // Properties
            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.MarketSegmentName)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Description)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("MarketSegment", "Fldr");
            this.Property(t => t.MarketSegmentID).HasColumnName("MarketSegmentID");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.MarketSegmentName).HasColumnName("MarketSegmentName");
            this.Property(t => t.Description).HasColumnName("Description");

            // Relationships
            this.HasRequired(t => t.Tenant)
                .WithMany(t => t.MarketSegments)
                .HasForeignKey(d => d.TenantID);

        }
    }
}
