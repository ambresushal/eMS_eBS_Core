using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class RelationKeyMap : EntityTypeConfiguration<RelationKey>
    {
        public RelationKeyMap()
        {
            // Primary Key
            this.HasKey(t => t.RelationKeyID);

            // Properties
            // Table & Column Mappings
            this.ToTable("RelationKeys", "DM");
            this.Property(t => t.RelationKeyID).HasColumnName("RelationKeyID");
            this.Property(t => t.RelationID).HasColumnName("RelationID");
            this.Property(t => t.LHSAttrID).HasColumnName("LHSAttrID");
            this.Property(t => t.RHSAttrID).HasColumnName("RHSAttrID");

            // Relationships
            this.HasRequired(t => t.Attribute)
                .WithMany(t => t.RelationKeys)
                .HasForeignKey(d => d.LHSAttrID);
            this.HasRequired(t => t.Attribute1)
                .WithMany(t => t.RelationKeys1)
                .HasForeignKey(d => d.RHSAttrID);
            this.HasRequired(t => t.ObjectRelation)
                .WithMany(t => t.RelationKeys)
                .HasForeignKey(d => d.RelationID);

        }
    }
}
