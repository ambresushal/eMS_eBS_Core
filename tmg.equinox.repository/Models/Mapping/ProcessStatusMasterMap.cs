using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ProcessStatusMasterMap : EntityTypeConfiguration<ProcessStatusMaster>
    {
        public ProcessStatusMasterMap()
        {
            this.HasKey(t => t.ProcessStatus1Up);
            this.ToTable("ProcessStatusMaster", "Setup");
            this.Property(t => t.ProcessStatus1Up).HasColumnName("ProcessStatus1Up");
            this.Property(t => t.ProcessStatusName).HasColumnName("ProcessStatusName");
        }
    }
}
