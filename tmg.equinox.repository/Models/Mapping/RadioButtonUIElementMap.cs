using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class RadioButtonUIElementMap : EntityTypeConfiguration<RadioButtonUIElement>
    {
        public RadioButtonUIElementMap()
        {
            // Primary Key
            this.HasKey(t => t.UIElementID);

            // Properties

            this.Property(t => t.OptionLabel)
                .HasMaxLength(20);

            this.Property(t => t.OptionLabelNo)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("RadioButtonUIElement", "UI");
            this.Property(t => t.UIElementID).HasColumnName("UIElementID");
            this.Property(t => t.UIElementTypeID).HasColumnName("UIElementTypeID");
            this.Property(t => t.OptionLabel).HasColumnName("OptionLabel");
            this.Property(t => t.DefaultValue).HasColumnName("DefaultValue");
            this.Property(t => t.IsYesNo).HasColumnName("IsYesNo");
            this.Property(t => t.OptionLabelNo).HasColumnName("OptionLabelNo");

            // Relationships
            //this.HasRequired(t => t.UIElement)
            //    .WithOptional(t => t.RadioButtonUIElement);
            this.HasRequired(t => t.UIElementType)
                .WithMany(t => t.RadioButtonUIElements)
                .HasForeignKey(d => d.UIElementTypeID);

        }
    }
}
