using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class PBPImportActivityLogMap : EntityTypeConfiguration<PBPImportActivityLog>
    {
        public PBPImportActivityLogMap()
        {
            this.HasKey(t => t.PBPImportActivityLog1Up);
            this.ToTable("PBPImportActivityLog", "Setup");
            this.Property(t => t.PBPImportActivityLog1Up).HasColumnName("PBPImportActivityLogID");
            this.Property(t => t.PBPImportQueueID).HasColumnName("PBPImportQueueID");
            this.Property(t => t.PBPImportBatchID).HasColumnName("PBPImportBatchID");
            this.Property(t => t.FileName).HasColumnName("FileName");
            this.Property(t => t.TableName).HasColumnName("TableName");
            this.Property(t => t.Message).HasColumnName("Message");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.UserErrorMessage).HasColumnName("UserErrorMessage");
        }
    }
}
