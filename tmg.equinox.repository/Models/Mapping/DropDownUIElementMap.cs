using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class DropDownUIElementMap : EntityTypeConfiguration<DropDownUIElement>
    {
        public DropDownUIElementMap()
        {
            // Primary Key
            this.HasKey(t => t.UIElementID);

            // Properties

            this.Property(t => t.SelectedValue)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("DropDownUIElement", "UI");
            this.Property(t => t.UIElementID).HasColumnName("UIElementID");
            this.Property(t => t.UIElementTypeID).HasColumnName("UIElementTypeID");
            this.Property(t => t.SelectedValue).HasColumnName("SelectedValue");
            this.Property(t => t.IsMultiSelect).HasColumnName("IsMultiSelect");
            this.Property(t => t.IsDropDownTextBox).HasColumnName("IsDropDownTextBox");
            this.Property(t => t.IsSortRequired).HasColumnName("IsSortRequired");
            this.Property(t => t.IsDropDownFilterable).HasColumnName("IsDropDownFilterable");
            // Relationships
            //this.HasRequired(t => t.UIElement)
            //    .WithOptional(t => t.DropDownUIElement);
            this.HasRequired(t => t.UIElementType)
                .WithMany(t => t.DropDownUIElements)
                .HasForeignKey(d => d.UIElementTypeID);

        }
    }
}
