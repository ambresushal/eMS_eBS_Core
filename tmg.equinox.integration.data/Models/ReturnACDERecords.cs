using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnACDERecords:Entity
    {
        public int ACDE_DISTINCT_ID { get; set; }
        public string ACDE_ACC_TYPE { get; set; }
        public short ACAC_ACC_NO { get; set; }
        public string ACDE_DESC { get; set; }
        public string Acronym { get; set; }
        public string PERIOD_IND { get; set; }
    }
}
