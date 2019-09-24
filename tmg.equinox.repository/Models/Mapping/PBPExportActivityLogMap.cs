using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class PBPExportActivityLogMap : EntityTypeConfiguration<PBPExportActivityLog>
    {
        public PBPExportActivityLogMap()
        {
            this.HasKey(t => t.PBPExportActivityLogID);
            this.ToTable("PBPExportActivityLog", "PBP");
            this.Property(t => t.PBPExportActivityLogID).HasColumnName("PBPExportActivityLogID");
            this.Property(t => t.PBPExportQueueID).HasColumnName("PBPExportQueueID");
            this.Property(t => t.TableName).HasColumnName("TableName");
            this.Property(t => t.Message).HasColumnName("Message");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
        }
    }
}
