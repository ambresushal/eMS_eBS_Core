using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ObjectTreeMap : EntityTypeConfiguration<ObjectTree>
    {
        public ObjectTreeMap()
        {
            // Primary Key
            this.HasKey(t => t.TreeID);

            // Properties
            // Table & Column Mappings
            this.ToTable("ObjectTree", "DM");
            this.Property(t => t.TreeID).HasColumnName("TreeID");
            this.Property(t => t.ParentOID).HasColumnName("ParentOID");
            this.Property(t => t.RootOID).HasColumnName("RootOID");
            this.Property(t => t.RelationID).HasColumnName("RelationID");
            this.Property(t => t.VersionID).HasColumnName("VersionID");

            // Relationships
            this.HasOptional(t => t.ObjectDefinition)
                .WithMany(t => t.ObjectTrees)
                .HasForeignKey(d => d.ParentOID);
            this.HasRequired(t => t.ObjectDefinition1)
                .WithMany(t => t.ObjectTrees1)
                .HasForeignKey(d => d.RootOID);
            this.HasOptional(t => t.ObjectRelation)
                .WithMany(t => t.ObjectTrees)
                .HasForeignKey(d => d.RelationID);
            this.HasRequired(t => t.ObjectVersion)
                .WithMany(t => t.ObjectTrees)
                .HasForeignKey(d => d.VersionID);

        }
    }
}
