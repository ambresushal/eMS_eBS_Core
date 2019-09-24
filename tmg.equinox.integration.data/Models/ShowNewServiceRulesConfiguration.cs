using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class ShowNewServiceRulesConfiguration : Entity
    {
        public string ProductID { get; set; }
        public string SESE_ID { get; set; }
        public string BenefitSet { get; set; }
        public string SESERuleCategory { get; set; }
        public bool IsAltRule { get; set; }
        public string Covered { get; set; }
        public string Disallowed_Message { get; set; }
        public string SESE_CM_IND { get; set; }
        public string SESE_PA_AMT_REQ { get; set; }
        public string SESE_PA_UNIT_REQ { get; set; }
        public string SESE_PA_PROC_REQ { get; set; }
        public string SESE_VALID_SEX { get; set; }
        public string SESE_SEX_EXCD_ID { get; set; }
        public Int16? SESE_MIN_AGE { get; set; }
        public Int16? SESE_MAX_AGE { get; set; }
        public string SESE_AGE_EXCD_ID { get; set; }
        public string SESE_COV_TYPE { get; set; }
        public string SESE_COV_EXCD_ID { get; set; }
        public string SESE_RULE_TYPE { get; set; }
        public string SESE_CALC_IND { get; set; }
        public string SERL_DESC { get; set; }
        public string SESE_OPTS { get; set; }
        public Int16? WMDS_SEQ_NO { get; set; }
        public decimal? SESE_MAX_CPAY_PCT { get; set; }
        public string SESE_FSA_REIMB_IND { get; set; }
        public string SESE_HSA_REIMB_IND { get; set; }
        public string SESE_HRA_DED_IND { get; set; }
        public string SESE_MAX_CPAY_ACT_NVL { get; set; }
        public string SESE_CPAY_EXCD_ID_NVL { get; set; }
        public string Specialty { get; set; }
        public Int16? SETR_TIER_NO { get; set; }
        public decimal? SETR_ALLOW_AMT { get; set; }
        public Int16? SETR_ALLOW_CTR { get; set; }
        public decimal? SETR_COPAY_AMT { get; set; }
        public decimal? SETR_COIN_PCT { get; set; }
        public Int16? ACAC_ACC_NO { get; set; }
        public string SETR_OPTS { get; set; }
        public string SESP_PEN_IND { get; set; }
        public string SESP_PEN_CALC_IND { get; set; }
        public decimal? SESP_PEN_AMT { get; set; }
        public decimal? SESP_PEN_PCT { get; set; }
        public decimal? SESP_PEN_MAX_AMT { get; set; }
        public string EXCD_ID { get; set; }
        public Int16? SETROther_TIER_NO { get; set; }
        public decimal? SETROther_ALLOW_AMT { get; set; }
        public Int16? SETROther_ALLOW_CTR { get; set; }
        public decimal? SETROther_COPAY_AMT { get; set; }
        public decimal? SETROther_COIN_PCT { get; set; }
        public Int16? ACACOther_ACC_NO { get; set; }
        public string SETROther_OPTS { get; set; }
    }
}