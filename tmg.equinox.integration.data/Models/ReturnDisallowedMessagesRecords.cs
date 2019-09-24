using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnDisallowedMessagesRecords : Entity
    {
        //public string SESE_ID { get; set; }
        //public string SESE_RULE { get; set; }
        //public Int16 SESR_WT_CTR { get; set; }
        public string SESE_DESC { get; set; }
        public string SESE_DIS_EXCD_ID { get; set; }
        public string SESE_ID { get; set; }
    }
}
