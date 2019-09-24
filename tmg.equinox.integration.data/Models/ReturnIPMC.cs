using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnIPMC : Entity
    {
        public int IPMC_DISTINCT_ID { get; set; }
        public string DESCRIPTION { get; set; }
        public string IPMC_UM_IND { get; set; }
        public double IPMC_UM_AUTH_AMT { get; set; }
        public int IPMC_UM_UNITS_WAIVE { get; set; }
        public int IPMC_AUTH_WV_DAYS { get; set; }
        public int IPCD_LOW { get; set; }
        public int IPCD_HIGH { get; set; }
    }
}
