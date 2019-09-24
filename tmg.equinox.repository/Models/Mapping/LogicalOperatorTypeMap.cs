using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class LogicalOperatorTypeMap : EntityTypeConfiguration<LogicalOperatorType>
    {
        public LogicalOperatorTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.LogicalOperatorTypeID);

            // Properties
            this.Property(t => t.LogicalOperatorTypeCode)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.DisplayText)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.Description)
                .HasMaxLength(200);

            this.Property(t => t.AddedBy)
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("LogicalOperatorType", "UI");
            this.Property(t => t.LogicalOperatorTypeID).HasColumnName("LogicalOperatorTypeID");
            this.Property(t => t.LogicalOperatorTypeCode).HasColumnName("LogicalOperatorTypeCode");
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
