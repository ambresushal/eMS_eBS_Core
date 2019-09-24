using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class LimitServiceDataSHDWMap : EntityTypeConfiguration<LimitServiceDataSHDW>
    {
        public LimitServiceDataSHDWMap()
        {
            this.HasKey(t => t.PDPD_ID);
            this.HasKey(t => t.SESE_ID);

            this.ToTable("LTSESHDWModeldata", "ModelData");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.BenefitSet).HasColumnName("BenefitSet");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
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
