using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class AttributeMap : EntityTypeConfiguration<Attribute>
    {
        public AttributeMap()
        {
            // Primary Key
            this.HasKey(t => t.AttrID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.NameCamelcase)
                .HasMaxLength(100);

            this.Property(t => t.AttrType)
                .HasMaxLength(50);

            this.Property(t => t.Cardinality)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.EditRegex)
                .HasMaxLength(90);

            this.Property(t => t.Formatter)
                .HasMaxLength(90);
         

            // Table & Column Mappings
            this.ToTable("Attribute", "DM").Ignore(t => t.UIElementID);
            this.Property(t => t.AttrID).HasColumnName("AttrID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.NameCamelcase).HasColumnName("NameCamelcase");
            this.Property(t => t.AttrType).HasColumnName("AttrType");
            this.Property(t => t.Cardinality).HasColumnName("Cardinality");
            this.Property(t => t.Length).HasColumnName("Length");
            this.Property(t => t.Precision).HasColumnName("Precision");
            this.Property(t => t.EditRegex).HasColumnName("EditRegex");
            this.Property(t => t.Formatter).HasColumnName("Formatter");
            this.Property(t => t.Synthetic).HasColumnName("Synthetic");
            this.Property(t => t.DefaultValue).HasColumnName("DefaultValue");
        }
    }
}
