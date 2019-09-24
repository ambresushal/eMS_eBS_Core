using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class LimitDiagnosisDataSHDWMap:EntityTypeConfiguration<LimitDiagnosisDataSHDW>
    {
        public LimitDiagnosisDataSHDWMap()
        {
            this.HasKey(t => t.PDPD_ID);

            this.ToTable("LTIDSHDWModeldata", "ModelData");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.BenefitSet).HasColumnName("BenefitSet");
            this.Property(t => t.ACDE_DESC).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.IDCD_ID_REL).HasColumnName("DEDE_DESC");
            this.Property(t => t.IDCD_TYPE).HasColumnName("DEDE_RULE");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");
        }
    }
}
