using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class CalendarUIElementMap : EntityTypeConfiguration<CalendarUIElement>
    {
        public CalendarUIElementMap()
        {
            // Primary Key
            this.HasKey(t => t.UIElementID);

            // Properties

            // Table & Column Mappings
            this.ToTable("CalendarUIElement", "UI");
            this.Property(t => t.UIElementID).HasColumnName("UIElementID");
            this.Property(t => t.UIElementTypeID).HasColumnName("UIElementTypeID");
            this.Property(t => t.MinDate).HasColumnName("MinDate");
            this.Property(t => t.MaxDate).HasColumnName("MaxDate");
            this.Property(t => t.DefaultDate).HasColumnName("DefaultDate");

            // Relationships
            //this.HasRequired(t => t.UIElement)
            //    .WithOptional(t => t.CalendarUIElement);
            this.HasRequired(t => t.UIElementType)
                .WithMany(t => t.CalendarUIElements)
                .HasForeignKey(d => d.UIElementTypeID);

        }
    }
}
