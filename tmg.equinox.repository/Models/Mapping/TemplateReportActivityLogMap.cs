using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class TemplateReportActivityLogMap:EntityTypeConfiguration<TemplateReportActivityLog>
    {
        public TemplateReportActivityLogMap()
        {
            this.HasKey(t=>t.ActivityLoggerID);

            this.ToTable("TemplateReportActivityLog","TmplRpt");
            this.Property(t=>t.ActivityLoggerID).HasColumnName("ActivityLoggerID");
            this.Property(t => t.TemplateReportVersionID).HasColumnName("TemplateReportVersionID");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");

            this.HasRequired(t => t.TemplateReportVersion)
                .WithMany(t => t.TemplateReportActivityLogs)
                .HasForeignKey(t => t.TemplateReportVersionID);
        }
    }
}
