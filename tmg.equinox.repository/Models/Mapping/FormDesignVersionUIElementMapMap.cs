using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FormDesignVersionUIElementMapMap : EntityTypeConfiguration<FormDesignVersionUIElementMap>
    {
        public FormDesignVersionUIElementMapMap()
        {
            // Primary Key
            this.HasKey(t => t.FormDesignVersionUIElementMapID);

            // Properties
            // Table & Column Mappings
            this.ToTable("FormDesignVersionUIElementMap", "UI");
            this.Property(t => t.FormDesignVersionUIElementMapID).HasColumnName("FormDesignVersionUIElementMapID");
            this.Property(t => t.FormDesignVersionID).HasColumnName("FormDesignVersionID");
            this.Property(t => t.UIElementID).HasColumnName("UIElementID");
            this.Property(t => t.EffectiveDate).HasColumnName("EffectiveDate");
            this.Property(t => t.EffectiveDateOfRemoval).HasColumnName("EffectiveDateOfRemoval");
            this.Property(t => t.Operation).HasColumnName("Operation");

            // Relationships
            this.HasRequired(t => t.UIElement)
                .WithMany(t => t.FormDesignVersionUIElementMaps)
                .HasForeignKey(d => d.UIElementID);

        }
    }
}
