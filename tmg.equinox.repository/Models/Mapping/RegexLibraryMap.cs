using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class RegexLibraryMap : EntityTypeConfiguration<RegexLibrary>
    {
        public RegexLibraryMap()
        {
            // Primary Key
            this.HasKey(t => t.LibraryRegexID);

            // Properties
            this.Property(t => t.LibraryRegexName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.RegexValue)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.AddedBy)
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);
            this.Property(t => t.MaskExpression)               
                .HasMaxLength(200);


            // Table & Column Mappings
            this.ToTable("RegexLibrary", "UI");
            this.Property(t => t.LibraryRegexID).HasColumnName("LibraryRegexID");
            this.Property(t => t.LibraryRegexName).HasColumnName("LibraryRegexName");
            this.Property(t => t.RegexValue).HasColumnName("RegexValue");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.MaskExpression).HasColumnName("MaskExpression");
            this.Property(t => t.Placeholder).HasColumnName("Placeholder");
            this.Property(t => t.Message).HasColumnName("Message");
        }
    }
}
