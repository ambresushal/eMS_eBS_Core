using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class ProcessStatusMasterDataMap : EntityTypeConfiguration<ProcessStatusMaster>
    {
        public ProcessStatusMasterDataMap()
        {
            this.HasKey(t => t.ProcessStatus1Up);
            this.ToTable("ProcessStatusMaster", "Setup");
            this.Property(t => t.ProcessStatus1Up).HasColumnName("ProcessStatus1Up");
            this.Property(t => t.ProcessStatusName).HasColumnName("ProcessStatusName");
        }
    }
}
