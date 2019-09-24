using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class SESRDataMap : EntityTypeConfiguration<SESRData>
    {
        public SESRDataMap()
        {
            this.HasKey(t => new { t.SESE_ID, t.SERL_REL_ID, t.SESR_WT_CTR, t.ProcessGovernance1up });

            this.ToTable("SESRModelData", "ModelData");
            this.Property(t => t.SERL_REL_ID).HasColumnName("SERL_REL_ID");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.SESR_WT_CTR).HasColumnName("SESR_WT_CTR");
            this.Property(t => t.SESR_OPTS).HasColumnName("SESR_OPTS");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");
        }
    }
}
