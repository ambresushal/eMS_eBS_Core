using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnLTIDRecords : Entity
    {
        public int LTLT_DISTINCT_ID { get; set; }
        public Int16 ACAC_ACC_NO { get; set; }
        public string LTLT_DESC { get; set; }
        public string IDCD_ID_REL { get; set; }
        public string ACDE_DESC { get; set; }
        public string IDCD_TYPE { get; set; }
    }
}
