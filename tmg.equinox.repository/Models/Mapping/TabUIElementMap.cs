using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class TabUIElementMap : EntityTypeConfiguration<TabUIElement>
    {
        public TabUIElementMap()
        {
            // Primary Key
            this.HasKey(t => t.UIElementID);

            // Properties
            
            // Table & Column Mappings
            this.ToTable("TabUIElement", "UI");
            this.Property(t => t.UIElementID).HasColumnName("UIElementID");
            this.Property(t => t.UIElementTypeID).HasColumnName("UIElementTypeID");
            this.Property(t => t.ChildCount).HasColumnName("ChildCount");
            this.Property(t => t.LayoutTypeID).HasColumnName("LayoutTypeID");

            // Relationships
            this.HasRequired(t => t.LayoutType)
                .WithMany(t => t.TabUIElements)
                .HasForeignKey(d => d.LayoutTypeID);
            //this.HasRequired(t => t.UIElement)
            //    .WithOptional(t => t.TabUIElement);
            this.HasRequired(t => t.UIElementType)
                .WithMany(t => t.TabUIElements)
                .HasForeignKey(d => d.UIElementTypeID);

        }
    }
}
