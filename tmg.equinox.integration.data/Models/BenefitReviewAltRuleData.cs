using System;
using System.Collections.Generic;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class BenefitReviewAltRuleData : Entity
    {
        public string PDPD_ID { get; set; }
        public string BenefitSet { get; set; }
        public string SESE_ID { get; set; }
        public Int16? SETR_TIER_No { get; set; }
        public decimal? SETR_COPAY_AMT { get; set; }
        public decimal? SETR_COIN_PCT { get; set; }
        public decimal? SETR_ALLOW_AMT { get; set; }
        public Int16? SETR_ALLOW_CTR { get; set; }
        public string ACAC_ACC_NO { get; set; }
        public string SETR_OPTS { get; set; }
        public string SESE_RULE_ALT { get; set; }
        public string BenefitCategory1 { get; set; }
        public string BenefitCategory2 { get; set; }
        public string BenefitCategory3 { get; set; }
        public string POS { get; set; }
        public string SESP_PEN_IND { get; set; }
        public string SESP_PEN_CALC_IND { get; set; }
        public decimal? SESP_PEN_AMT { get; set; }
        public decimal? SESP_PEN_PCT { get; set; }
        public decimal? SESP_PEN_MAX_AMT { get; set; }
        public string EXCD_ID { get; set; }
        public string SESP_OPTS { get; set; }
        public string Covered { get; set; }
        public string SERL_DESC { get; set; }
        public string Disallowed_Message { get; set; }
        public int ProcessGovernance1up { get; set; }
        public string SESE_RULE_ALT_COND { get; set; }
        public string DeductibleAccumulator { get; set; }

        public BenefitReviewAltRuleData Clone()
        {
            return this.MemberwiseClone() as BenefitReviewAltRuleData;
        }
    }
}
