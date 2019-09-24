using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class SETR : Entity
    {
        public string SESE_ID { get; set; }
        public string SESE_RULE { get; set; }
        public short SETR_TIER_NO { get; set; }
        public Nullable<decimal> SETR_ALLOW_AMT { get; set; }
        public Nullable<short> SETR_ALLOW_CTR { get; set; }
        public Nullable<decimal> SETR_COPAY_AMT { get; set; }
        public Nullable<decimal> SETR_COIN_PCT { get; set; }
        public Nullable<short> ACAC_ACC_NO { get; set; }
        public string SETR_OPTS { get; set; }
        public Nullable<short> SETR_LOCK_TOKEN { get; set; }
        public Nullable<System.DateTime> ATXR_SOURCE_ID { get; set; }
        public Nullable<System.DateTime> SYS_LAST_UPD_DTM { get; set; }
        public string SYS_USUS_ID { get; set; }
        public string SYS_DBUSER_ID { get; set; }
    }
}
