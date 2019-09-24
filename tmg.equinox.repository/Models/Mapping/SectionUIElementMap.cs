using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class SectionUIElementMap : EntityTypeConfiguration<SectionUIElement>
    {
        public SectionUIElementMap()
        {
            // Primary Key
            this.HasKey(t => t.UIElementID);

            // Properties

            // Table & Column Mappings
            this.ToTable("SectionUIElement", "UI");
            this.Property(t => t.UIElementID).HasColumnName("UIElementID");
            this.Property(t => t.UIElementTypeID).HasColumnName("UIElementTypeID");
            this.Property(t => t.ChildCount).HasColumnName("ChildCount");
            this.Property(t => t.LayoutTypeID).HasColumnName("LayoutTypeID");
            this.Property(t => t.DataSourceID).HasColumnName("DataSourceID");
            this.Property(t => t.CustomHtml).HasColumnName("CustomHtml");

            // Relationships
            this.HasOptional(t => t.DataSource)
                .WithMany(t => t.SectionUIElements)
                .HasForeignKey(d => d.DataSourceID);
            this.HasRequired(t => t.LayoutType)
                .WithMany(t => t.SectionUIElements)
                .HasForeignKey(d => d.LayoutTypeID);
            //this.HasRequired(t => t.UIElement)
            //    .WithOptional(t => t.SectionUIElement);
            this.HasRequired(t => t.UIElementType)
                .WithMany(t => t.SectionUIElements)
                .HasForeignKey(d => d.UIElementTypeID);

        }
    }
}
