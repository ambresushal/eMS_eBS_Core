using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class OperatorTypeMap : EntityTypeConfiguration<OperatorType>
    {
        public OperatorTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.OperatorTypeID);

            // Properties
            this.Property(t => t.OperatorTypeCode)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.DisplayText)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.Description)
                .HasMaxLength(200);

            this.Property(t => t.AddedBy)
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("OperatorType", "UI");
            this.Property(t => t.OperatorTypeID).HasColumnName("OperatorTypeID");
            this.Property(t => t.OperatorTypeCode).HasColumnName("OperatorTypeCode");
            this.Property(t => t.DisplayText).HasColumnName("DisplayText");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
