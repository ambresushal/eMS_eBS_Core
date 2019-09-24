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
    public class QHPReportingQueueDetailsMap : EntityTypeConfiguration<QHPReportingQueueDetails>
    {
        public QHPReportingQueueDetailsMap()
        {
            // Primary Key
            this.HasKey(t => t.QueueDetailID);

            // Table & Column Mappings
            this.ToTable("QHPReportingQueueDetails", "Qhp");
            this.Property(t => t.QueueDetailID).HasColumnName("QueueDetailID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.QueueID).HasColumnName("QueueID").IsRequired();
            this.Property(t => t.FolderID).HasColumnName("FolderID");
            this.Property(t => t.FolderVersionID).HasColumnName("FolderVersionID");
            this.Property(t => t.FormInstanceID).HasColumnName("FormInstanceID");
            this.Property(t => t.FormDesignID).HasColumnName("FormDesignID");

            //////// Relationships
            this.HasRequired<QHPReportingQueue>(t => t.Queue)
                .WithMany(r => r.QueueDetails)
                .HasForeignKey<int>(t => t.QueueID).WillCascadeOnDelete(false);
        }
    }
}
