using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ACCMREP : Entity
    {
        public int ACCMId { get; set; }
        public string PDPD_ID { get; set; }
        public string ACCM_TYPE { get; set; }
        public System.DateTime ACCM_EFF_DT { get; set; }
        public int ACCM_SEQ_NO { get; set; }
        public System.DateTime ACCM_TERM_DT { get; set; }
        public int ACAC_ACC_NO { get; set; }
        public string ACCM_DESC { get; set; }
        public string ACCM_PFX { get; set; }
        public int ACCM_LOCK_TOKEN { get; set; }
        public System.DateTime ATXR_SOURCE_ID { get; set; }
        public string BatchID { get; set; }
        public int Action { get; set; }
        public string Hashcode { get; set; }
    }
}
