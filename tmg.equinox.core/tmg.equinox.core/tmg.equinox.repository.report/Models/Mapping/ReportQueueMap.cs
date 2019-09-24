using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.repository.models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ReportQueueMap : EntityTypeConfiguration<ReportQueue>
    {
        public ReportQueueMap()
        {
            // Primary Key
            this.HasKey(t => t.ReportQueueId);

            this.ToTable("ReportQueue", "Rpt");
            this.Property(t => t.ReportQueueId).HasColumnName("ReportQueueId");
            this.Property(t => t.ReportId).HasColumnName("ReportId");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.CompletionDate).HasColumnName("CompletionDate");
            this.Property(t => t.FileName).HasColumnName("FileName");
            this.Property(t => t.DestinationPath).HasColumnName("DestinationPath");
            this.Property(t => t.JobId).HasColumnName("JobId");
            this.Property(t => t.ErrorMessage).HasColumnName("ErrorMessage");

        }

    }
}
