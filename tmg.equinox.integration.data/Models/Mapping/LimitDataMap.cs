using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class LimitDataMap : EntityTypeConfiguration<LimitData>
    {
        public LimitDataMap()
        {
            this.HasKey(t => new { t.PDPD_ID, t.BenefitSet, t.ACAC_ACC_NO, t.ACDE_DESC, t.ProcessGovernance1up });

            this.ToTable("LTLTModeldata", "ModelData");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.BenefitSet).HasColumnName("BenefitSet");
            this.Property(t => t.ACDE_DESC).HasColumnName("ACDE_DESC");
            this.Property(t => t.LTLT_AMT1).HasColumnName("LTLT_AMT1");
            this.Property(t => t.LTLT_AMT2).HasColumnName("LTLT_AMT2");
            this.Property(t => t.LTLT_DAYS).HasColumnName("LTLT_DAYS");
            this.Property(t => t.LTLT_DESC).HasColumnName("LTLT_DESC");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.LTLT_CAT).HasColumnName("LTLT_CAT");
            this.Property(t => t.LTLT_LEVEL).HasColumnName("LTLT_LEVEL");
            this.Property(t => t.LTLT_PERIOD_IND).HasColumnName("LTLT_PERIOD_IND");
            this.Property(t => t.LTLT_RULE).HasColumnName("LTLT_RULE");
            this.Property(t => t.LTLT_IX_IND).HasColumnName("LTLT_IX_IND");
            this.Property(t => t.LTLT_IX_TYPE).HasColumnName("LTLT_IX_TYPE");
            this.Property(t => t.EXCD_ID).HasColumnName("EXCD_ID");
            this.Property(t => t.LTLT_SAL_IND).HasColumnName("LTLT_SAL_IND");
            this.Property(t => t.WMDS_SEQ_NO).HasColumnName("WMDS_SEQ_NO");
            this.Property(t => t.EBCLSelection).HasColumnName("EBCLSelection");
            this.Property(t => t.LTLT_EXCL_DED_IND_NVL).HasColumnName("LTLT_EXCL_DED_IND_NVL");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");
            this.Property(t => t.IDCD_ID_REL).HasColumnName("IDCD_ID_REL");
            this.Property(t => t.IDCD_TYPE).HasColumnName("IDCD_TYPE");
            this.Property(t => t.LTIP_IPCD_ID_LOW).HasColumnName("LTIP_IPCD_ID_LOW");
            this.Property(t => t.LTIP_IPCD_ID_HIGH).HasColumnName("LTIP_IPCD_ID_HIGH");
            this.Property(t => t.PRPR_MCTR_TYPE).HasColumnName("PRPR_MCTR_TYPE");
        }
    }
}
