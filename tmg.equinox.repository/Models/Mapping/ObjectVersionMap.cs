using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ObjectVersionMap : EntityTypeConfiguration<ObjectVersion>
    {
        public ObjectVersionMap()
        {
            // Primary Key
            this.HasKey(t => t.VersionID);

            // Properties
            this.Property(t => t.VersionName)
                .IsRequired()
                .HasMaxLength(55);

            // Table & Column Mappings
            this.ToTable("ObjectVersion", "DM");
            this.Property(t => t.VersionID).HasColumnName("VersionID");
            this.Property(t => t.VersionName).HasColumnName("VersionName");
            this.Property(t => t.EffectiveFrom).HasColumnName("EffectiveFrom");
            this.Property(t => t.EffectiveTo).HasColumnName("EffectiveTo");
        }
    }
}
