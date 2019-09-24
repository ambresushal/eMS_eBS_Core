using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FormInstanceDataMapMap : EntityTypeConfiguration<FormInstanceDataMap>
    {
        public FormInstanceDataMapMap()
        {
            // Primary Key
            this.HasKey(t => t.FormInstanceDataMapID);

            // Properties
            // Table & Column Mappings
            this.ToTable("FormInstanceDataMap", "Fldr");
            this.Property(t => t.FormInstanceDataMapID).HasColumnName("FormInstanceDataMapID");
            this.Property(t => t.FormInstanceID).HasColumnName("FormInstanceID");
            this.Property(t => t.ObjectInstanceID).HasColumnName("ObjectInstanceID");
            this.Property(t => t.FormData).HasColumnName("FormData");
            this.Property(t => t.CompressJsonData).HasColumnName("CompressJsonData");

            // Relationships
            this.HasRequired(t => t.FormInstance)
                .WithMany(t => t.FormInstanceDataMaps)
                .HasForeignKey(d => d.FormInstanceID);

        }
    }
}
