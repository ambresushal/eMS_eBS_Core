using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnLTPRRecords : Entity
    {
        public int LTLT_DISTINCT_ID { get; set; }
        public Int16 ACAC_ACC_NO { get; set; }
        public string LTLT_DESC { get; set; }
        public string PRPR_MCTR_TYPE { get; set; }
        public string ACDE_DESC { get; set; }
    }
}
