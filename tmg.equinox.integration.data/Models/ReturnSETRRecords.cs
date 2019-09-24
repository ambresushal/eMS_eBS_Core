using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnSETRRecords:Entity
    {
        public int SETR_DISTINCT_ID { get; set; }
        public string SESE_RULE { get; set; }
        public string SESE_DESC { get; set; }
        public short SETR_TIER_NO { get; set; }
        public Nullable<decimal> SETR_ALLOW_AMT { get; set; }
        public Nullable<short> SETR_ALLOW_CTR { get; set; }
        public Nullable<decimal> SETR_COPAY_AMT { get; set; }
        public Nullable<decimal> SETR_COIN_PCT { get; set; }
        public Nullable<short> ACAC_ACC_NO { get; set; }
        public string SETR_OPTS { get; set; }
        public string RULEID { get; set; }
    }
}
