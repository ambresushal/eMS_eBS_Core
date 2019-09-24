using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnLTLTRecords : Entity
    {
        public int LTLT_DISTINCT_ID { get; set; }
        public Int16 ACAC_ACC_NO { get; set; }
        public string LTLT_DESC { get; set; }
        public string LTLT_CAT { get; set; }
        public string LTLT_LEVEL { get; set; }
        public string LTLT_PERIOD_IND { get; set; }
        public string LTLT_RULE { get; set; }
        public string ACDE_DESC { get; set; }
        //public string LTLT_OPTS { get; set; }
        public string LTLT_IX_IND { get; set; }
        public string LTLT_IX_TYPE { get; set; }
        public string EXCD_ID { get; set; }
        public string LTLT_SAL_IND { get; set; }
        public Int16 WMDS_SEQ_NO { get; set; }
        public string ACDE_ACC_TYPE { get; set; }
        public string BenefitSetName { get; set; }
        public decimal LTLT_AMT1 { get; set; }
        public decimal LTLT_AMT2 { get; set; }
        public Int16 LTLT_DAYS { get; set; }
    }
}
