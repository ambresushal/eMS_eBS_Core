using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class LTLTJsonData : Entity
    {
        public string ProductID { get; set; }
        public string BenefitSet { get; set; }
        public int ACAC_ACC_NO { get; set; }
        public string LTLT_CAT { get; set; }
        public string LTLT_LEVEL { get; set; }
        public string LTLT_PERIOD_IND { get; set; }
        public string LTLT_RULE { get; set; }
        public string LTLT_IX_IND { get; set; }
        public string LTLT_IX_TYPE { get; set; }
        public string EXCD_ID { get; set; }
        public float LTLT_AMT1 { get; set; }
        public float LTLT_AMT2 { get; set; }
        public string LTLT_OPTS { get; set; }
        public string LTLT_SAL_IND { get; set; }
        public int LTLT_DAYS { get; set; }
        public int WMDS_SEQ_NO { get; set; }
        public string LTLT_EXCL_DED_IND_NVL { get; set; }


        public string IDCD_ID_REL { get; set; }
        public string IDCD_TYPE { get; set; }
        public string LTIP_IPCD_ID_LOW { get; set; }
        public string LTIP_IPCD_ID_HIGH { get; set; }
        public string PRPR_MCTR_TYPE { get; set; }
    }
}
