using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FormVersionObjectVersionMapMap : EntityTypeConfiguration<FormVersionObjectVersionMap>
    {
        public FormVersionObjectVersionMapMap()
        {
            // Primary Key
            this.HasKey(t => t.FormVersionObjectVersionMap1);

            //// Properties
            //this.Property(t => t.FormVersionObjectVersionMap1)
            //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("FormVersionObjectVersionMap", "UI");
            this.Property(t => t.FormVersionObjectVersionMap1).HasColumnName("FormVersionObjectVersionMap");
            this.Property(t => t.FormDesignVersionID).HasColumnName("FormDesignVersionID");
            this.Property(t => t.ObjectVersionID).HasColumnName("ObjectVersionID");

            // Relationships
            this.HasRequired(t => t.ObjectVersion)
                .WithMany(t => t.FormVersionObjectVersionMaps)
                .HasForeignKey(d => d.ObjectVersionID);
            this.HasRequired(t => t.FormDesignVersion)
                .WithMany(t => t.FormVersionObjectVersionMaps)
                .HasForeignKey(d => d.FormDesignVersionID);

        }
    }
}
