using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.repository.models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ReportSettingMap : EntityTypeConfiguration<ReportSetting>
    {
        public ReportSettingMap()
        {
            // Primary Key
            this.HasKey(t => t.ReportId);

            this.ToTable("ReportSetting", "Rpt");
            this.Property(t => t.ReportId).HasColumnName("ReportId");
            this.Property(t => t.ReportName).HasColumnName("ReportName");
            this.Property(t => t.ReportTemplatePath).HasColumnName("ReportTemplatePath");
            this.Property(t => t.OutputPath).HasColumnName("OutputPath");
            this.Property(t => t.ReportNameFormat).HasColumnName("ReportNameFormat");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.SQLstatement).HasColumnName("SQLStatement");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.Visible).HasColumnName("Visible");
            Ignore(t => t.isMapping);
            Ignore(t => t.mappings);

        }

    }
}