using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class ReportTypeMap : EntityTypeConfiguration<ReportType>
    {
        public ReportTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.ReportTypeID);

            // Properties
            this.Property(t => t.ReportTypeName).IsRequired().HasMaxLength(100);
            this.Property(t => t.AddedBy).IsRequired().HasMaxLength(20);
            this.Property(t => t.UpdatedBy).HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("ReportTypeMap", "Rpt");
            this.Property(t => t.ReportTypeID).HasColumnName("ReportTypeID");
            this.Property(t => t.ReportTypeName).HasColumnName("ReportTypeName");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");

        }
    }
}
