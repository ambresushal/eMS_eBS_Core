using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class SETRAltJsonData : Entity
    {
        public string ProductID { get; set; }
        public string SESE_ID { get; set; }
        public string BenefitSet { get; set; }
        public int SETR_TIER_NO { get; set; }
        public decimal SETR_ALLOW_AMT { get; set; }
        public int SETR_ALLOW_CTR { get; set; }
        public float SETR_COPAY_AMT { get; set; }
        public float SETR_COIN_PCT { get; set; }
        public int ACAC_ACC_NO { get; set; }
        public string SETR_OPTS { get; set; }
    }
}
