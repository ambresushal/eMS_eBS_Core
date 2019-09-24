using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ObjectDefinitionMap : EntityTypeConfiguration<ObjectDefinition>
    {
        public ObjectDefinitionMap()
        {
            // Primary Key
            this.HasKey(t => t.OID);

            // Properties
            this.Property(t => t.ObjectName)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("ObjectDefinition", "DM");
            this.Property(t => t.OID).HasColumnName("OID");
            this.Property(t => t.ObjectName).HasColumnName("ObjectName");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.Locked).HasColumnName("Locked");
        }
    }
}
