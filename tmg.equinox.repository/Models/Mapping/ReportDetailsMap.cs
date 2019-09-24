using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ReportDetailsMap : EntityTypeConfiguration<ReportDetails>
    {
        public ReportDetailsMap()
        {
            // Primary Key
            this.HasKey(t => t.ReportDetailsId);

            this.ToTable("ReportDetails", "WCRpt");
            this.Property(t => t.ReportDetailsId).HasColumnName("ReportDetailsId");
            this.Property(t => t.ReportQueueId).HasColumnName("ReportQueueId");
            this.Property(t => t.FolderId).HasColumnName("FolderId");
            this.Property(t => t.FolderVersionId).HasColumnName("FolderVersionId");
            this.Property(t => t.Status).HasColumnName("Status");

        }

    }
}

