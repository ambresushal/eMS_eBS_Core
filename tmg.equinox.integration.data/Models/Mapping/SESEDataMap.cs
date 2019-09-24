using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class SESEDataMap : EntityTypeConfiguration<SESEData>
    {
        public SESEDataMap()
        {
            this.HasKey(t => new { t.SESE_ID, t.ProcessGovernance1up });

            this.ToTable("SESEIDListModeldata", "ModelData");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");
        }
    }
}
