using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class ReportFormatTypeMap : EntityTypeConfiguration<ReportFormatType>
    {
        public ReportFormatTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.ReportFFormatTypeID);

            // Properties
            this.Property(t => t.ReportFormatTypeName).IsRequired().HasMaxLength(50);
            this.Property(t => t.AddedBy).IsRequired().HasMaxLength(20);
            this.Property(t => t.UpdatedBy).HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("ReportFormatType", "Rpt");
            this.Property(t => t.ReportFFormatTypeID).HasColumnName("ReportFFormatTypeID");
            this.Property(t => t.ReportFormatTypeName).HasColumnName("ReportFormatTypeName");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
        }
    }
}
