using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class LayoutTypeMap : EntityTypeConfiguration<LayoutType>
    {
        public LayoutTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.LayoutTypeID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.LayoutTypeCode)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.DisplayText)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ClassName)
                .HasMaxLength(50);

            this.Property(t => t.AddedBy)
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("LayoutType", "UI");
            this.Property(t => t.LayoutTypeID).HasColumnName("LayoutTypeID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.LayoutTypeCode).HasColumnName("LayoutTypeCode");
            this.Property(t => t.DisplayText).HasColumnName("DisplayText");
            this.Property(t => t.ClassName).HasColumnName("ClassName");
            this.Property(t => t.ColumnCount).HasColumnName("ColumnCount");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
