using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class BenefitSummaryDetailsDataMap: EntityTypeConfiguration<BenefitSummaryDetailsData>
    {
        public BenefitSummaryDetailsDataMap()
        {
            this.HasKey(t => new { t.PDPD_ID, t.BSDL_TYPE, t.BSDL_NTWK_IND, t.ProcessGovernance1up });

            this.ToTable("BSDLModelData", "ModelData");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.BSDL_TYPE).HasColumnName("BSDL_TYPE");
            this.Property(t => t.BSDL_NTWK_IND).HasColumnName("BSDL_NTWK_IND");
           // this.Property(t => t.BSDL_EFF_DT).HasColumnName("BSDL_EFF_DT");
            this.Property(t => t.BSDL_COPAY_AMT).HasColumnName("BSDL_COPAY_AMT");
            this.Property(t => t.BSDL_DEDE_AMT).HasColumnName("BSDL_DEDE_AMT");
            this.Property(t => t.BSDL_COIN_PCT).HasColumnName("BSDL_COIN_PCT");
            this.Property(t => t.BSDL_LTLT_AMT).HasColumnName("BSDL_LTLT_AMT");
            //this.Property(t => t.BSDL_TERM_DT).HasColumnName("BSDL_TERM_DT");
            this.Property(t => t.BSDL_LT_TYPE).HasColumnName("BSDL_LT_TYPE");
            this.Property(t => t.BSDL_LT_PERIOD).HasColumnName("BSDL_LT_PERIOD");
            this.Property(t => t.BSDL_LT_COUNTER).HasColumnName("BSDL_LT_COUNTER");
            this.Property(t => t.BSDL_TIER).HasColumnName("BSDL_TIER");
            this.Property(t => t.BSDL_COV_IND).HasColumnName("BSDL_COV_IND");
            this.Property(t => t.BSDL_STOPLOSS_AMT).HasColumnName("BSDL_STOPLOSS_AMT");
            this.Property(t => t.BSDL_STOPLOSS_TYPE).HasColumnName("BSDL_STOPLOSS_TYPE");
            this.Property(t => t.BSDL_BEG_MMDD).HasColumnName("BSDL_BEG_MMDD");
            this.Property(t => t.BSDL_USER_LABEL1).HasColumnName("BSDL_USER_LABEL1");
            this.Property(t => t.BSDL_USER_DATA1).HasColumnName("BSDL_USER_DATA1");
            this.Property(t => t.BSDL_USER_LABEL2).HasColumnName("BSDL_USER_LABEL2");
            this.Property(t => t.BSDL_USER_DATA2).HasColumnName("BSDL_USER_DATA2");
            this.Property(t => t.BSDL_USER_LABEL3).HasColumnName("BSDL_USER_LABEL3");
            this.Property(t => t.BSDL_USER_DATA3).HasColumnName("BSDL_USER_DATA3");
            this.Property(t => t.BSDL_USER_LABEL4).HasColumnName("BSDL_USER_LABEL4");
            this.Property(t => t.BSDL_USER_DATA4).HasColumnName("BSDL_USER_DATA4");
            this.Property(t => t.BSDL_USER_LABEL5).HasColumnName("BSDL_USER_LABEL5");
            this.Property(t => t.BSDL_USER_DATA5).HasColumnName("BSDL_USER_DATA5");
            this.Property(t => t.BSDL_USER_LABEL6).HasColumnName("BSDL_USER_LABEL6");
            this.Property(t => t.BSDL_USER_DATA6).HasColumnName("BSDL_USER_DATA6");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");
        }
    }
}
