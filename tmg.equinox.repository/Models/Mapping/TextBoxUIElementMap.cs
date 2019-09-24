using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class TextBoxUIElementMap : EntityTypeConfiguration<TextBoxUIElement>
    {
        public TextBoxUIElementMap()
        {
            // Primary Key
            this.HasKey(t => t.UIElementID);

            // Properties

            this.Property(t => t.DefaultValue)
                .HasMaxLength(4000);

            // Table & Column Mappings
            this.ToTable("TextBoxUIElement", "UI");
            this.Property(t => t.UIElementID).HasColumnName("UIElementID");
            this.Property(t => t.UIElementTypeID).HasColumnName("UIElementTypeID");
            this.Property(t => t.IsMultiline).HasColumnName("IsMultiline");
            this.Property(t => t.DefaultValue).HasColumnName("DefaultValue");
            this.Property(t => t.MaxLength).HasColumnName("MaxLength");
            this.Property(t => t.IsLabel).HasColumnName("IsLabel");
            this.Property(t => t.SpellCheck).HasColumnName("SpellCheck");

            // Relationships
            //this.HasRequired(t => t.UIElement)
            //    .WithOptional(t => t.TextBoxUIElement);
            this.HasRequired(t => t.UIElementType)
                .WithMany(t => t.TextBoxUIElements)
                .HasForeignKey(d => d.UIElementTypeID);

        }
    }
}
