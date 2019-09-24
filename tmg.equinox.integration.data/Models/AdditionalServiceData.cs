using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class AdditionalServiceData : Entity
    {
        public string PDPD_ID { get; set; }
        public string SESE_ID { get; set; }
        public string BenefitSet { get; set; }
        public string SESE_FSA_REIMB_IND { get; set; }
        public string SESE_HSA_REIMB_IND { get; set; }
        public string SESE_HRA_DED_IND { get; set; }
        public decimal SESE_MAX_CPAY_PCT { get; set; }
        public string SESE_CALC_IND { get; set; }
        public string SESE_VALID_SEX { get; set; }
        public string SESE_SEX_EXCD_ID { get; set; }
        public Int16 SESE_MIN_AGE { get; set; }
        public Int16 SESE_MAX_AGE { get; set; }
        public string SESE_AGE_EXCD_ID { get; set; }
        public string SESE_COV_TYPE { get; set; }
        public string SESE_COV_EXCD_ID { get; set; }
        public string SESE_CM_IND { get; set; }
        public Int16 WMDS_SEQ_NO { get; set; }
        public string SESE_PA_AMT_REQ { get; set; }
        public string SESE_PA_UNIT_REQ { get; set; }
        public string SESE_PA_PROC_REQ { get; set; }
        public string Covered { get; set; }
        public string SERL_DESC { get; set; }
        public string Disallowed_Message { get; set; }
        public string SESE_MAX_CPAY_ACT_NVL { get; set; }
        public string SESE_CPAY_EXCD_ID_NVL { get; set; }
        public string BenefitCategory1 { get; set; }
        public string BenefitCategory2 { get; set; }
        public string BenefitCategory3 { get; set; }
        public string POS { get; set; }
        public string SESE_OPTS { get; set; }
        public string RuleCategory { get; set; }
        public string SESE_RULE_TYPE { get; set; }
        public int ProcessGovernance1up { get; set; }
        public string Specialty { get; set; } 

        public AdditionalServiceData Clone()
        {
            return this.MemberwiseClone() as AdditionalServiceData;
        }
    }
}
