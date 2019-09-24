using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnSEDS:Entity
    {
        public int SedsId { get; set; }
        public string SESE_ID { get; set; }
        public string seds_desc { get; set; }
        public string seds_type { get; set; }
        public string sese_id_xlow { get; set; }
        public string seds_desc_xlow { get; set; }

        public Int64 RowNum { get; set; }

        public string BENEFIT_CATEGORY1 { get; set; }
        public string BENEFIT_CATEGORY2 { get; set; }
        public string BENEFIT_CATEGORY3 { get; set; }

        public string BenefitCategory1 { get; set; }
        public string BenefitCategory2 { get; set; }
        public string BenefitCategory3 { get; set; }
        public string POS { get; set; }

        public string SESE_CM_IND { get; set; }
        public string SESE_PA_AMT_REQ { get; set; }
        public string SESE_PA_UNIT_REQ { get; set; }
        public string SESE_PA_PROC_REQ { get; set; }
        public string SESE_VALID_SEX { get; set; }
        public string SESE_SEX_EXCD_ID { get; set; }
        public short SESE_MIN_AGE { get; set; }
        public short SESE_MAX_AGE { get; set; }
        public string SESE_AGE_EXCD_ID { get; set; }
        public string SESE_COV_TYPE { get; set; }
        public string SESE_COV_EXCD_ID { get; set; }
        //public string SESE_RULE_TYPE { get; set; }
        public string SESE_CALC_IND { get; set; }
        public string SESE_OPTS { get; set; }
        public short WMDS_SEQ_NO { get; set; }
        public string SESE_DIS_EXCD_ID { get; set; }
        public decimal SESE_MAX_CPAY_PCT { get; set; }
        public string SESE_FSA_REIMB_IND { get; set; }
        public string SESE_HSA_REIMB_IND { get; set; }
        public string SESE_HRA_DED_IND { get; set; }
        public string SESE_MAX_CPAY_ACT_NVL { get; set; }
        public string SESE_CPAY_EXCD_ID_NVL { get; set; }
        public string Comment { set; get; }
    }
}
