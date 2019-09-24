using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class TemplateReportMap : EntityTypeConfiguration<TemplateReport>
    {
        public TemplateReportMap()
        {
            // Primary Key
            this.HasKey(t => t.TemplateReportID);

            // Properties
            this.Property(t => t.TemplateReportName).IsRequired().HasMaxLength(50);
            this.Property(t => t.AddedBy).IsRequired().HasMaxLength(20);
            this.Property(t => t.UpdatedBy).HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("TemplateReport", "TmplRpt");
            this.Property(t => t.TemplateReportID).HasColumnName("TemplateReportID");
            this.Property(t => t.TemplateReportName).HasColumnName("TemplateReportName");
            //this.Property(t => t.LocationID).HasColumnName("LocationID");

            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");

            this.Property(t => t.ReportDescription).HasColumnName("ReportDescription");

            this.Property(t => t.ReportType).HasColumnName("ReportType");
            this.Property(t => t.HelpText).HasColumnName("HelpText");
            this.Property(t => t.IsActive).HasColumnName("IsActive");


            //this.HasRequired(t => t.TemplateReportLocation)
            //    .WithMany(t => t.TemplateReports)
            //    .HasForeignKey(t => t.LocationID);
        }
    }
}
