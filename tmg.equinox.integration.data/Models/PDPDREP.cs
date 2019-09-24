using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.data.Models
{
    public partial class PDPDREP : Entity
    {        
        public string PDPD_ID { get; set; }
        public System.DateTime PDPD_EFF_DT { get; set; }
        public System.DateTime PDPD_TERM_DT { get; set; }
        public string PDPD_RISK_IND { get; set; }
        public string LOBD_ID { get; set; }
        public string LOBD_ALT_RISK_ID { get; set; }
        public string PDPD_ACC_SFX { get; set; }
        public string PDPD_OPTS { get; set; }
        public string PDPD_CAP_POP_LVL { get; set; }
        public int PDPD_CAP_RET_MOS { get; set; }
        public string PDPD_MCTR_CCAT { get; set; }
        public int PDPD_LOCK_TOKEN { get; set; }
        public System.DateTime ATXR_SOURCE_ID { get; set; }
        public Nullable<System.DateTime> SYS_LAST_UPD_DTM { get; set; }
        public string SYS_USUS_ID { get; set; }
        public string SYS_DBUSER_ID { get; set; }        
        public string PDPD_ACC_SHDW_SFX_NVL { get; set; }
        public string Hashcode { get; set; }
    }
}
