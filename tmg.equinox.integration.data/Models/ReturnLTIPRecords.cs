using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnLTIPRecords : Entity
    {
        public int LTLT_DISTINCT_ID { get; set; }
        public Int16 ACAC_ACC_NO { get; set; }
        public string LTLT_DESC { get; set; }
        public string LTIP_IPCD_ID_LOW { get; set; }
        public string LTIP_IPCD_ID_HIGH { get; set; }
        public string ACDE_DESC { get; set; }
    }
}
