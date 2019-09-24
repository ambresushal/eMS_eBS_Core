using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class RepeaterUIElementMap : EntityTypeConfiguration<RepeaterUIElement>
    {
        public RepeaterUIElementMap()
        {
            // Primary Key
            this.HasKey(t => t.UIElementID);

            // Properties

            // Table & Column Mappings
            this.ToTable("RepeaterUIElement", "UI");
            this.Property(t => t.UIElementID).HasColumnName("UIElementID");
            this.Property(t => t.UIElementTypeID).HasColumnName("UIElementTypeID");
            this.Property(t => t.LayoutTypeID).HasColumnName("LayoutTypeID");
            this.Property(t => t.ChildCount).HasColumnName("ChildCount");
            this.Property(t => t.DataSourceID).HasColumnName("DataSourceID");
            this.Property(t => t.LoadFromServer).HasColumnName("LoadFromServer");
            this.Property(t => t.IsDataRequired).HasColumnName("IsDataRequired");
            this.Property(t => t.AllowBulkUpdate).HasColumnName("AllowBulkUpdate");

            // Properties for Configuring Param Query Features 
            this.Property(t => t.DisplayTopHeader).HasColumnName("DisplayTopHeader");
            this.Property(t => t.DisplayTitle).HasColumnName("DisplayTitle");
            this.Property(t => t.FrozenColCount).HasColumnName("FrozenColCount");
            this.Property(t => t.FrozenRowCount).HasColumnName("FrozenRowCount");
            this.Property(t => t.AllowPaging).HasColumnName("AllowPaging");
            this.Property(t => t.RowsPerPage).HasColumnName("RowsPerPage");
            this.Property(t => t.AllowExportToExcel).HasColumnName("AllowExportToExcel");
            this.Property(t => t.AllowExportToCSV).HasColumnName("AllowExportToCSV");
            this.Property(t => t.FilterMode).HasColumnName("FilterMode");

            // Relationships
            this.HasOptional(t => t.DataSource)
                .WithMany(t => t.RepeaterUIElements)
                .HasForeignKey(d => d.DataSourceID);
            this.HasRequired(t => t.LayoutType)
                .WithMany(t => t.RepeaterUIElements)
                .HasForeignKey(d => d.LayoutTypeID);
            //this.HasRequired(t => t.UIElement)
            //    .WithOptional(t => t.RepeaterUIElement);

        }
    }
}
