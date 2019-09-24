using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class TemplateReportVersionMap : EntityTypeConfiguration<TemplateReportVersion>
    {
        public TemplateReportVersionMap()
        {

            //Primary Key
            this.HasKey(t => t.TemplateReportVersionID);

            //Properties
            this.Property(t => t.Description).HasMaxLength(250);
            this.Property(t => t.VersionNumber).IsRequired().HasMaxLength(10);
            this.Property(t => t.HelpText).HasMaxLength(250);
            this.Property(t => t.AddedBy).IsRequired().HasMaxLength(20);
            this.Property(t => t.UpdatedBy).HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("TemplateReportVersion", "TmplRpt");
            this.Property(t => t.TemplateReportVersionID).HasColumnName("TemplateReportVersionID");
            this.Property(t => t.TemplateReportID).HasColumnName("TemplateReportID");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.VersionNumber).HasColumnName("VersionNumber");
            this.Property(t => t.EffectiveDate).IsRequired().HasColumnName("EffectiveDate");
            this.Property(t => t.CancelDate).HasColumnName("CancelDate");
            this.Property(t => t.HelpText).HasColumnName("HelpText");
            this.Property(t => t.TemplateFileName).HasColumnName("TemplateFileName");
            this.Property(t => t.IsVisible).HasColumnName("IsVisible");
            this.Property(t => t.Location).HasColumnName("Location");
            this.Property(t => t.IsReleased).HasColumnName("IsReleased");
            this.Property(t => t.ReportFormatTypeID).IsRequired().HasColumnName("ReportFormatTypeID");
            this.Property(t => t.TenantID).IsRequired().HasColumnName("TenantID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");

            this.HasRequired(t => t.TemplateReport)
                .WithMany(t => t.TemplateReportVersions)
                .HasForeignKey(t => t.TemplateReportID);

            this.HasRequired(t => t.ReportFormatType)
                .WithMany(t => t.TemplateReportVersions)
                .HasForeignKey(t => t.ReportFormatTypeID);




        }
    }
}
