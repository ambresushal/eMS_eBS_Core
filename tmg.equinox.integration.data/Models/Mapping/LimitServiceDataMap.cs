using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class LimitServiceDataMap: EntityTypeConfiguration<LimitServiceData>
    {
        public LimitServiceDataMap()
        {
            this.HasKey(t => new { t.PDPD_ID, t.BenefitSet, t.ACDE_DESC, t.SESE_ID, t.ProcessGovernance1up });

            this.ToTable("LTSEModeldata", "ModelData");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.BenefitSet).HasColumnName("BenefitSet");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.ACDE_DESC).HasColumnName("ACDE_DESC");
            this.Property(t => t.LTSE_WT_CTR).HasColumnName("LTSE_WT_CTR");
            this.Property(t => t.BenefitCategory1).HasColumnName("BenefitCategory1");
            this.Property(t => t.BenefitCategory2).HasColumnName("BenefitCategory2");
            this.Property(t => t.BenefitCategory3).HasColumnName("BenefitCategory3");
            this.Property(t => t.POS).HasColumnName("POS");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up"); 
        }
    }
}
