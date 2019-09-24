using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FormReportMap : EntityTypeConfiguration<FormReport>
    {
        public FormReportMap()
        {
            // Primary Key
            this.HasKey(t => t.ReportID);

            // Properties
            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("FormReport", "Rpt");
            this.Property(t => t.ReportID).HasColumnName("ReportID");
            this.Property(t => t.TenantID).IsRequired().HasColumnName("TenantID");
            this.Property(t => t.AddedDate).IsRequired().HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).IsRequired().HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");

            // Relationships
            this.HasRequired(t => t.Tenant)
                .WithMany(t => t.FormReports)
                .HasForeignKey(d => d.TenantID);


        }

    }
}
