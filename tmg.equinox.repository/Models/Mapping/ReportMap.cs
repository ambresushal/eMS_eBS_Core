using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ReportMap : EntityTypeConfiguration<Report>
    {
        public ReportMap()
        {
            // Primary Key
            this.HasKey(t => t.ReportId);


            this.ToTable("Report", "Rpt");
            this.Property(t => t.ReportId).HasColumnName("ReportId");
            this.Property(t => t.ReportName).HasColumnName("ReportName");
           

        }

    }
}

