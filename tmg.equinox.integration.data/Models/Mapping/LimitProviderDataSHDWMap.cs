using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class LimitProviderDataSHDWMap : EntityTypeConfiguration<LimitProviderDataSHDW>  
    {
        public LimitProviderDataSHDWMap()
        {
            this.HasKey(t => t.PDPD_ID);

            this.ToTable("LTPRSHDWModeldata", "ModelData");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.BenefitSet).HasColumnName("BenefitSet");
            this.Property(t => t.ACDE_DESC).HasColumnName("ACDE_DESC");
            this.Property(t => t.PRPR_MCTR_TYPE).HasColumnName("PRPR_MCTR_TYPE");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");
        }
    }
}
