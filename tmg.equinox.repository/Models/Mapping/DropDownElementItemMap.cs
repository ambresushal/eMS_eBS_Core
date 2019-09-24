using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class DropDownElementItemMap : EntityTypeConfiguration<DropDownElementItem>
    {
        public DropDownElementItemMap()
        {
            // Primary Key
            this.HasKey(t => t.ItemID);

            // Properties
            this.Property(t => t.Value)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.AddedBy)
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.DisplayText)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("DropDownElementItem", "UI");
            this.Property(t => t.ItemID).HasColumnName("ItemID");
            this.Property(t => t.UIElementID).HasColumnName("UIElementID");
            this.Property(t => t.Value).HasColumnName("Value");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.DisplayText).HasColumnName("DisplayText");
            this.Property(t => t.Sequence).HasColumnName("Sequence");

            // Relationships
            this.HasRequired(t => t.DropDownUIElement)
                .WithMany(t => t.DropDownElementItems)
                .HasForeignKey(d => d.UIElementID);

        }
    }
}
