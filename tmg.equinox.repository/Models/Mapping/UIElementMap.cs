using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class UIElementMap : EntityTypeConfiguration<UIElement>
    {
        public UIElementMap()
        {
            // Primary Key
            this.HasKey(t => t.UIElementID);

            // Properties
            this.Property(t => t.UIElementName)
                .HasMaxLength(100);

            this.Property(t => t.Label)
                .HasMaxLength(1000);

            this.Property(t => t.HelpText)
                .HasMaxLength(1000);

            this.Property(t => t.AddedBy)
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            
            this.Property(t => t.GeneratedName)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("UIElement", "UI");
            this.Property(t => t.UIElementID).HasColumnName("UIElementID");
            this.Property(t => t.UIElementName).HasColumnName("UIElementName");
            this.Property(t => t.Label).HasColumnName("Label");
            this.Property(t => t.ParentUIElementID).HasColumnName("ParentUIElementID");
            this.Property(t => t.IsContainer).HasColumnName("IsContainer");
            this.Property(t => t.Enabled).HasColumnName("Enabled");
            this.Property(t => t.Visible).HasColumnName("Visible");
            this.Property(t => t.Sequence).HasColumnName("Sequence");
            this.Property(t => t.RequiresValidation).HasColumnName("RequiresValidation");
            this.Property(t => t.HelpText).HasColumnName("HelpText");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.FormID).HasColumnName("FormID");
            this.Property(t => t.UIElementDataTypeID).HasColumnName("UIElementDataTypeID");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.HasCustomRule).HasColumnName("HasCustomRule");
            this.Property(t => t.CustomRule).HasColumnName("CustomRule");
            this.Property(t => t.GeneratedName).HasColumnName("GeneratedName");
            this.Property(t => t.DataSourceElementDisplayModeID).HasColumnName("DataSourceElementDisplayModeID");
            this.Property(t => t.CheckDuplicate).HasColumnName("CheckDuplicate");
            this.Property(t => t.ViewType).HasColumnName("ViewType");
            this.Property(t => t.ExtendedProperties).HasColumnName("ExtendedProperties");
            this.Property(t => t.IsStandard).HasColumnName("IsStandard");
			this.Property(t => t.IsSameSectionRuleSource).HasColumnName("IsSameSectionRuleSource");
            // Relationships
            this.HasRequired(t => t.ApplicationDataType)
                .WithMany(t => t.UIElements)
                .HasForeignKey(d => d.UIElementDataTypeID);
            this.HasOptional(t => t.DataSourceElementDisplayMode)
                .WithMany(t => t.UIElements)
                .HasForeignKey(d => d.DataSourceElementDisplayModeID);
            this.HasRequired(t => t.FormDesign)
                .WithMany(t => t.UIElements)
                .HasForeignKey(d => d.FormID);
            this.HasOptional(t => t.UIElement2)
                .WithMany(t => t.UIElement1)
                .HasForeignKey(d => d.ParentUIElementID);

        }
    }
}
