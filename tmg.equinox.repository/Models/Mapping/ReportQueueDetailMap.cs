using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ReportQueueDetailMap : EntityTypeConfiguration<ReportQueueDetail>
    {
        public ReportQueueDetailMap()
        {
            // Primary Key
            this.HasKey(t => t.ReportDetailsId);

            this.ToTable("ReportQueueDetail", "Rpt");
            this.Property(t => t.ReportDetailsId).HasColumnName("ReportDetailsId");
            this.Property(t => t.ReportQueueId).HasColumnName("ReportQueueId");
            this.Property(t => t.FolderId).HasColumnName("FolderId");
            this.Property(t => t.FolderVersionId).HasColumnName("FolderVersionId");
            this.Property(t => t.FormInstanceId).HasColumnName("FormInstanceId");
            this.Property(t => t.Status).HasColumnName("Status");
            this.HasRequired<ReportQueue>(t => t.ReportQueues)
                .WithMany(r => r.ReportQueueDetails)
                .HasForeignKey<int>(t => t.ReportQueueId).WillCascadeOnDelete(false);
        }

    }
}

