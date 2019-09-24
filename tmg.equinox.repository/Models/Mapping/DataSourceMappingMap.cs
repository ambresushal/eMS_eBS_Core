using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class DataSourceMappingMap : EntityTypeConfiguration<DataSourceMapping>
    {
        public DataSourceMappingMap()
        {
            // Primary Key
            this.HasKey(t => t.DataSourceMappingID);

            // Properties
            this.Property(t => t.DataSourceFilter)
                .HasMaxLength(200);

            this.Property(t => t.Value)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("DataSourceMapping", "UI");
            this.Property(t => t.DataSourceMappingID).HasColumnName("DataSourceMappingID");
            this.Property(t => t.UIElementID).HasColumnName("UIElementID");
            this.Property(t => t.DataSourceID).HasColumnName("DataSourceID");
            this.Property(t => t.MappedUIElementID).HasColumnName("MappedUIElementID");
            this.Property(t => t.IsPrimary).HasColumnName("IsPrimary");
            this.Property(t => t.DataSourceElementDisplayModeID).HasColumnName("DataSourceElementDisplayModeID");
            this.Property(t => t.DataSourceFilter).HasColumnName("DataSourceFilter");
            this.Property(t => t.DataCopyModeID).HasColumnName("DataCopyModeID");
            this.Property(t => t.FormDesignID).HasColumnName("FormDesignID");
            this.Property(t => t.FormDesignVersionID).HasColumnName("FormDesignVersionID");
            this.Property(t => t.IsKey).HasColumnName("IsKey");
            this.Property(t => t.DataSourceOperatorID).HasColumnName("DataSourceOperatorID");
            this.Property(t => t.Value).HasColumnName("Value");
            this.Property(t => t.DataSourceModeID).HasColumnName("DataSourceModeID");
            this.Property(t => t.IncludeChild).HasColumnName("IncludeChild");
            // Relationships
            this.HasOptional(t => t.DataCopyMode)
                .WithMany(t => t.DataSourceMappings)
                .HasForeignKey(d => d.DataCopyModeID);
            this.HasRequired(t => t.DataSource)
                .WithMany(t => t.DataSourceMappings)
                .HasForeignKey(d => d.DataSourceID);
            this.HasOptional(t => t.DataSourceElementDisplayMode)
                .WithMany(t => t.DataSourceMappings)
                .HasForeignKey(d => d.DataSourceElementDisplayModeID);
            this.HasOptional(t => t.DataSourceOperatorMapping)
                .WithMany(t => t.DataSourceMappings)
                .HasForeignKey(d => d.DataSourceOperatorID);
            this.HasRequired(t => t.FormDesign)
                .WithMany(t => t.DataSourceMappings)
                .HasForeignKey(d => d.FormDesignID);
            this.HasRequired(t => t.FormDesignVersion)
                .WithMany(t => t.DataSourceMappings)
                .HasForeignKey(d => d.FormDesignVersionID);
            this.HasRequired(t => t.UIElement)
                .WithMany(t => t.DataSourceMappings)
                .HasForeignKey(d => d.UIElementID);
            this.HasRequired(t => t.UIElement1)
                .WithMany(t => t.DataSourceMappings1)
                .HasForeignKey(d => d.MappedUIElementID);

        }
    }
}
