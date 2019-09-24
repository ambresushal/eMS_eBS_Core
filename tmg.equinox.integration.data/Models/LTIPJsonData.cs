using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class LTIPJsonData : Entity
    {
        public string ProductID { get; set; }
        public string BenefitSet { get; set; }
        public int ACAC_ACC_NO { get; set; }
        public string LTIP_IPCD_ID_HIGH { get; set; }
        public string LTIP_IPCD_ID_LOW { get; set; }
    }
}
