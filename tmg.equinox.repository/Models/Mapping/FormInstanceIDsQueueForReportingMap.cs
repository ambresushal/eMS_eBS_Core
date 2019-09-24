using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FormInstanceIDsQueueForReportingMap : EntityTypeConfiguration<FormInstanceIDsQueueForReporting>
    {
        public FormInstanceIDsQueueForReportingMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);
            // Table & Column Mappings
            this.ToTable("FormInstanceIDsQueueForReporting", "Rpt");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.FormInstanceId).HasColumnName("FormInstanceId");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
