using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FormInstanceHistoryMap : EntityTypeConfiguration<FormInstanceHistory>
    {
        public FormInstanceHistoryMap()
        {
            // Primary Key
            this.HasKey(t => t.FormInstanceHistoryId);

            // Properties
            this.Property(t => t.FormInstanceID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.EnteredBy)
                .HasMaxLength(20);

            this.Property(t => t.TenantID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Action)
                .IsFixedLength()
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("FormInstanceHistory", "Arc");
            this.Property(t => t.FormData).HasColumnName("FormData");
            this.Property(t => t.FormInstanceID).HasColumnName("FormInstanceID");
            this.Property(t => t.EnteredBy).HasColumnName("EnteredBy");
            this.Property(t => t.EnteredDate).HasColumnName("EnteredDate");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.Action).HasColumnName("Action");
            this.Property(t => t.FormInstanceHistoryId).HasColumnName("FormInstanceHistoryId");

        }
    }
}
