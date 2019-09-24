using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class PDPDJsonData : Entity
    {
        public string PDPD_ID { get; set; }
        public string PDPD_RISK_IND { get; set; }
        public string LOBD_ID { get; set; }
        public string LOBD_ALT_RISK_ID { get; set; }
        public string PDPD_ACC_SFX  { get; set; }
        public string  PDPD_OPTS{ get; set; }
        public string  PDPD_CAP_POP_LVL{ get; set; }
        public int  PDPD_CAP_RET_MOS{ get; set; }
        public string  PDPD_MCTR_CCAT{ get; set; }
    }
}
