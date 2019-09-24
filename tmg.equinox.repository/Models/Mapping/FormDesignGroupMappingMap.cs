using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FormDesignGroupMappingMap : EntityTypeConfiguration<FormDesignGroupMapping>
    {
        public FormDesignGroupMappingMap()
        {
            // Primary Key
            this.HasKey(t => t.FormDesignGroupMappingID);

            // Properties
            // Table & Column Mappings
            this.ToTable("FormDesignGroupMapping", "UI");
            this.Property(t => t.FormDesignGroupMappingID).HasColumnName("FormDesignGroupMappingID");
            this.Property(t => t.FormDesignGroupID).HasColumnName("FormDesignGroupID");
            this.Property(t => t.FormID).HasColumnName("FormID");
            this.Property(t => t.Sequence).HasColumnName("Sequence");
            this.Property(t => t.AllowMultipleInstance).HasColumnName("AllowMultipleInstance");
            this.Property(t => t.AccessibleToRoles).HasColumnName("AccessibleToRoles");

            // Relationships
            this.HasRequired(t => t.FormDesign)
                .WithMany(t => t.FormDesignGroupMappings)
                .HasForeignKey(d => d.FormID);
            this.HasRequired(t => t.FormDesignGroup)
                .WithMany(t => t.FormDesignGroupMappings)
                .HasForeignKey(d => d.FormDesignGroupID);

        }
    }
}
