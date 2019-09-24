using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class TargetPropertyMap : EntityTypeConfiguration<TargetProperty>
    {
        public TargetPropertyMap()
        {
            // Primary Key
            this.HasKey(t => t.TargetPropertyID);

            // Properties
            this.Property(t => t.TargetPropertyName)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("TargetProperty", "UI");
            this.Property(t => t.TargetPropertyID).HasColumnName("TargetPropertyID");
            this.Property(t => t.TargetPropertyName).HasColumnName("TargetPropertyName");
        }
    }
}
