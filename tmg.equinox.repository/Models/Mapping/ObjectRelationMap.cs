using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ObjectRelationMap : EntityTypeConfiguration<ObjectRelation>
    {
        public ObjectRelationMap()
        {
            // Primary Key
            this.HasKey(t => t.RelationID);

            // Properties
            this.Property(t => t.RelationName)
                .HasMaxLength(100);

            this.Property(t => t.RelationNameCamelcase)
                .HasMaxLength(100);

            this.Property(t => t.Cardinality)
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("ObjectRelation", "DM");
            this.Property(t => t.RelationID).HasColumnName("RelationID");
            this.Property(t => t.RelatedObjectID).HasColumnName("RelatedObjectID");
            this.Property(t => t.RelationName).HasColumnName("RelationName");
            this.Property(t => t.RelationNameCamelcase).HasColumnName("RelationNameCamelcase");
            this.Property(t => t.Cardinality).HasColumnName("Cardinality");

            // Relationships
            this.HasOptional(t => t.ObjectDefinition)
                .WithMany(t => t.ObjectRelations)
                .HasForeignKey(d => d.RelatedObjectID);

        }
    }
}
