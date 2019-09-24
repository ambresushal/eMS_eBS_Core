using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ValidatorMap : EntityTypeConfiguration<Validator>
    {
        public ValidatorMap()
        {
            // Primary Key
            this.HasKey(t => t.ValidatorID);

            // Properties
            this.Property(t => t.Regex)
                .HasMaxLength(200);

            this.Property(t => t.AddedBy)
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.Message)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("Validator", "UI");
            this.Property(t => t.ValidatorID).HasColumnName("ValidatorID");
            this.Property(t => t.UIElementID).HasColumnName("UIElementID");
            this.Property(t => t.IsRequired).HasColumnName("IsRequired");
            this.Property(t => t.Regex).HasColumnName("Regex");
            this.Property(t => t.IsLibraryRegex).HasColumnName("IsLibraryRegex");
            this.Property(t => t.LibraryRegexID).HasColumnName("LibraryRegexID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.Message).HasColumnName("Message");
            this.Property(t => t.MaskExpression).HasColumnName("MaskExpression");
            this.Property(t => t.MaskFlag).HasColumnName("MaskFlag");

            // Relationships
            this.HasOptional(t => t.RegexLibrary)
                .WithMany(t => t.Validators)
                .HasForeignKey(d => d.LibraryRegexID);
            this.HasRequired(t => t.UIElement)
                .WithMany(t => t.Validators)
                .HasForeignKey(d => d.UIElementID);

        }
    }
}
