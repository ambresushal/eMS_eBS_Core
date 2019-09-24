using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ObjectVersionAttribXrefMap : EntityTypeConfiguration<ObjectVersionAttribXref>
    {
        public ObjectVersionAttribXrefMap()
        {
            // Primary Key
            this.HasKey(t => t.ObjVerID);

            // Properties
            // Table & Column Mappings
            this.ToTable("ObjectVersionAttribXref", "DM");
            this.Property(t => t.ObjVerID).HasColumnName("ObjVerID");
            this.Property(t => t.VersionID).HasColumnName("VersionID");
            this.Property(t => t.OID).HasColumnName("OID");
            this.Property(t => t.AttrID).HasColumnName("AttrID");

            // Relationships
            this.HasRequired(t => t.Attribute)
                .WithMany(t => t.ObjectVersionAttribXrefs)
                .HasForeignKey(d => d.AttrID);
            this.HasRequired(t => t.ObjectDefinition)
                .WithMany(t => t.ObjectVersionAttribXrefs)
                .HasForeignKey(d => d.OID);
            this.HasRequired(t => t.ObjectVersion)
                .WithMany(t => t.ObjectVersionAttribXrefs)
                .HasForeignKey(d => d.VersionID);

        }
    }
}
