using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ManualDataUpdatesMap : EntityTypeConfiguration<ManualDataUpdates>
    {
        public ManualDataUpdatesMap()
        {
            this.HasKey(t => t.ManualDataUpdateID);
            this.ToTable("ManualDataUpdates", "ODM");
            this.Property(t => t.ManualDataUpdateID).HasColumnName("ManualDataUpdateID");
            this.Property(t => t.QID).HasColumnName("QID");
            this.Property(t => t.ViewType).HasColumnName("ViewType");
            this.Property(t => t.DocumentPath).HasColumnName("DocumentPath");
            this.Property(t => t.DataValue).HasColumnName("DataValue");
            this.Property(t => t.IsArray).HasColumnName("IsArray");
        }
    }
}
