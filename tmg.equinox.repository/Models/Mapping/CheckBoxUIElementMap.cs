using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class CheckBoxUIElementMap : EntityTypeConfiguration<CheckBoxUIElement>
    {
        public CheckBoxUIElementMap()
        {
            // Primary Key
            this.HasKey(t => t.UIElementID);

            // Properties

            this.Property(t => t.OptionLabel)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("CheckBoxUIElement", "UI");
            this.Property(t => t.UIElementID).HasColumnName("UIElementID");
            this.Property(t => t.UIElementTypeID).HasColumnName("UIElementTypeID");
            this.Property(t => t.OptionLabel).HasColumnName("OptionLabel");
            this.Property(t => t.DefaultValue).HasColumnName("DefaultValue");

            // Relationships
            //this.HasRequired(t => t.UIElement)
            //    .WithOptional(t => t.CheckBoxUIElement);
            this.HasRequired(t => t.UIElementType)
                .WithMany(t => t.CheckBoxUIElements)
                .HasForeignKey(d => d.UIElementTypeID);

        }
    }
}
