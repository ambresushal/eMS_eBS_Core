using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.facet.data.Models
{
    public class AdditionalServiceAltDataSHDWMap:EntityTypeConfiguration<AdditionalServiceAltRuleDataSHDW>
    {
        public AdditionalServiceAltDataSHDWMap()
        {
            this.HasKey(t => new { t.PDPD_ID, t.SESE_ID, t.BenefitSet });            

            this.ToTable("AdditionalServiceAltSHDWModeldata", "ModelData");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.BenefitSet).HasColumnName("BenefitSet");
            this.Property(t => t.SESE_FSA_REIMB_IND).HasColumnName("SESE_FSA_REIMB_IND");
            this.Property(t => t.SESE_HSA_REIMB_IND).HasColumnName("SESE_HSA_REIMB_IND");
            this.Property(t => t.SESE_HRA_DED_IND).HasColumnName("SESE_HRA_DED_IND");
            this.Property(t => t.SESE_MAX_CPAY_PCT).HasColumnName("SESE_MAX_CPAY_PCT");
            this.Property(t => t.SESE_CALC_IND).HasColumnName("SESE_CALC_IND");
            this.Property(t => t.SESE_VALID_SEX).HasColumnName("SESE_VALID_SEX");
            this.Property(t => t.SESE_SEX_EXCD_ID).HasColumnName("SESE_SEX_EXCD_ID");
            this.Property(t => t.SESE_MIN_AGE).HasColumnName("SESE_MIN_AGE");
            this.Property(t => t.SESE_MAX_AGE).HasColumnName("SESE_MAX_AGE");
            this.Property(t => t.SESE_AGE_EXCD_ID).HasColumnName("SESE_AGE_EXCD_ID");
            this.Property(t => t.SESE_COV_TYPE).HasColumnName("SESE_COV_TYPE");
            this.Property(t => t.SESE_COV_EXCD_ID).HasColumnName("SESE_COV_EXCD_ID");
            this.Property(t => t.SESE_CM_IND).HasColumnName("SESE_CM_IND");
            this.Property(t => t.WMDS_SEQ_NO).HasColumnName("WMDS_SEQ_NO");
            this.Property(t => t.SESE_PA_AMT_REQ).HasColumnName("SESE_PA_AMT_REQ");
            this.Property(t => t.SESE_PA_UNIT_REQ).HasColumnName("SESE_PA_UNIT_REQ");
            this.Property(t => t.SESE_PA_PROC_REQ).HasColumnName("SESE_PA_PROC_REQ");
            this.Property(t => t.Covered).HasColumnName("Covered");
            this.Property(t => t.SERL_DESC).HasColumnName("SERL_DESC");
            this.Property(t => t.Disallowed_Message).HasColumnName("Disallowed_Message");
            this.Property(t => t.SESE_CPAY_EXCD_ID_NVL).HasColumnName("SESE_CPAY_EXCD_ID_NVL");
            this.Property(t => t.SESE_MAX_CPAY_ACT_NVL).HasColumnName("SESE_MAX_CPAY_ACT_NVL");
            this.Property(t => t.BenefitCategory1).HasColumnName("BenefitCategory1");
            this.Property(t => t.BenefitCategory2).HasColumnName("BenefitCategory2");
            this.Property(t => t.BenefitCategory3).HasColumnName("BenefitCategory3");
            this.Property(t => t.POS).HasColumnName("POS");
            this.Property(t => t.SESE_OPTS).HasColumnName("SESE_OPTS");
            this.Property(t => t.RuleCategory).HasColumnName("SESERuleCategoryID");
            this.Property(t => t.SESE_RULE_TYPE).HasColumnName("SESE_RULE_TYPE");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");
        }
    }
}
