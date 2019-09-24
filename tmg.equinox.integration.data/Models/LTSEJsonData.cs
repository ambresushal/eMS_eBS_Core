using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class LTSEJsonData : Entity
    {
        public string ProductID { get; set; }
        public string BenefitSet { get; set; }
        public int ACAC_ACC_NO { get; set; }
        public string SESE_ID { get; set; }
        public int LTSE_WT_CTR { get; set; }
    }
}
