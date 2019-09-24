using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class LTIDJsonData : Entity
    {
        public string ProductID { get; set; }
        public string BenefitSet { get; set; }
        public int ACAC_ACC_NO { get; set; }
        public string IDCD_ID_REL { get; set; }
        public string IDCD_TYPE { get; set; }
    }
}
