using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    class CollateralProcessQueueStatusMap : EntityTypeConfiguration<CollateralProcessQueueStatus>
    {
        public CollateralProcessQueueStatusMap()
        {
            this.HasKey(t => t.CollateralProcessQueueStatus1Up);
            this.ToTable("CollateralProcessQueueStatus", "Setup");
            this.Property(t => t.CollateralProcessQueue1Up).HasColumnName("CollateralProcessQueue1Up");
            this.Property(t => t.CollateralProcessQueueStatus1Up).HasColumnName("CollateralProcessQueueStatus1Up");
            this.Property(t => t.Message).HasColumnName("Message");            
        }
    }
}
