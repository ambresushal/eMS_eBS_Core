using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class PBPDataMapMap : EntityTypeConfiguration<PBPDataMap>
    {
        public PBPDataMapMap()
        {
            this.HasKey(t => t.PBPDataMapId);

            this.ToTable("PBPDataMap", "PBP");

            this.Property(t => t.PBPDataMapId).HasColumnName("PBPDataMapId");
            this.Property(t => t.QID).HasColumnName("QID");
            this.Property(t => t.TableName).HasColumnName("TableName");
            this.Property(t => t.FieldName).HasColumnName("FieldName");
            this.Property(t => t.JsonData).HasColumnName("JsonData");
            this.Property(t => t.PBPImportQueueID).HasColumnName("PBPImportQueueID");
        }
    }
}
