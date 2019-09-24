using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class AlternateUIElementLabelMap : EntityTypeConfiguration<AlternateUIElementLabel>
    {
        public AlternateUIElementLabelMap()
        {
            // Primary Key
            this.HasKey(t => t.AlternateLabelID);

            // Properties
            this.Property(t => t.AlternateLabel)
                .HasMaxLength(1000);

            // Table & Column Mappings
            this.ToTable("AlternateUIElementLabel", "UI");
            this.Property(t => t.AlternateLabelID).HasColumnName("AlternateLabelID");
            this.Property(t => t.AlternateLabel).HasColumnName("AlternateLabel");
            this.Property(t => t.FormDesignID).HasColumnName("FormDesignID");
            this.Property(t => t.FormDesignVersionID).HasColumnName("FormDesignVersionID");
            this.Property(t => t.UIElementID).HasColumnName("UIElementID");

            // Relationships
            this.HasRequired(t => t.FormDesign)
                .WithMany(t => t.AlternateUIElementLabels)
                .HasForeignKey(d => d.FormDesignID);
            this.HasRequired(t => t.FormDesignVersion)
                .WithMany(t => t.AlternateUIElementLabels)
                .HasForeignKey(d => d.FormDesignVersionID);
        }
    }
}
