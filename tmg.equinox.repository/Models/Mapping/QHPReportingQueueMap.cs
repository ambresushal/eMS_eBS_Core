using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class QHPReportingQueueMap : EntityTypeConfiguration<QHPReportingQueue>
    {
        public QHPReportingQueueMap()
        {
            // Primary Key
            this.HasKey(t => t.QueueID);

            // Table & Column Mappings
            this.ToTable("QHPReportingQueue", "Qhp");
            this.Property(t => t.QueueID).HasColumnName("QueueID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.QueuedDate).HasColumnName("QueuedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.DocumentLocation).HasColumnName("DocumentLocation");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Message).HasColumnName("Message");
            this.Property(t => t.OffExchangeOnly).HasColumnName("OffExchangeOnly");
        }
    }
}
