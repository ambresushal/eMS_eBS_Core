using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class ServiceTier0DataMap:EntityTypeConfiguration<ServiceTier0Data>
    {
        public ServiceTier0DataMap()
        {
            this.HasKey(t => new { t.PDPD_ID, t.SESE_ID, t.BenefitSet, t.ProcessGovernance1up });            

            this.ToTable("SETRModeldata", "ModelData");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.BenefitSet).HasColumnName("BenefitSet");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.SETR_TIER_No).HasColumnName("SETR_TIER_No");
            this.Property(t => t.SETR_COPAY_AMT).HasColumnName("SETR_COPAY_AMT");
            this.Property(t => t.SETR_COIN_PCT).HasColumnName("SETR_COIN_PCT");
            this.Property(t => t.SETR_ALLOW_AMT).HasColumnName("SETR_ALLOW_AMT");
            this.Property(t => t.SETR_ALLOW_CTR).HasColumnName("SETR_ALLOW_CTR");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.SETR_OPTS).HasColumnName("SETR_OPTS");
            this.Property(t => t.BenefitCategory1).HasColumnName("BenefitCategory1");
            this.Property(t => t.BenefitCategory2).HasColumnName("BenefitCategory2");
            this.Property(t => t.BenefitCategory3).HasColumnName("BenefitCategory3");
            this.Property(t => t.POS).HasColumnName("POS");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");

            this.Property(t => t.SESP_PEN_IND).HasColumnName("SESP_PEN_IND");
            this.Property(t => t.SESP_PEN_CALC_IND).HasColumnName("SESP_PEN_CALC_IND");
            this.Property(t => t.SESP_PEN_AMT).HasColumnName("SESP_PEN_AMT");
            this.Property(t => t.SESP_PEN_PCT).HasColumnName("SESP_PEN_PCT");
            this.Property(t => t.SESP_PEN_MAX_AMT).HasColumnName("SESP_PEN_MAX_AMT");
            this.Property(t => t.EXCD_ID).HasColumnName("EXCD_ID");            
            this.Property(t => t.SESP_OPTS).HasColumnName("SESP_OPTS");
            this.Property(t => t.SESE_COV_TYPE).HasColumnName("SESE_COV_TYPE");
            this.Property(t => t.SESE_DESC).HasColumnName("SESE_DESC");
            this.Property(t => t.SERLMessage).HasColumnName("SERLMessage");


        }
    }
}
