using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class PDBCTRANS : Entity
    {
        public string PDBC_PFX { get; set; }
        public string PDPD_ID { get; set; }
        public string PDBC_TYPE { get; set; }
        public System.DateTime PDBC_EFF_DT { get; set; }
        public System.DateTime PDBC_TERM_DT { get; set; }
        public string PDBC_OPTS { get; set; }
        //public short PDBC_LOCK_TOKEN { get; set; }
        //public System.DateTime ATXR_SOURCE_ID { get; set; }
        //public Nullable<System.DateTime> SYS_LAST_UPD_DTM { get; set; }
        //public string SYS_USUS_ID { get; set; }
        //public string SYS_DBUSER_ID { get; set; }
        public string BatchID { get; set; }
        public int Action { get; set; }
        //public string Hashcode { get; set; }
    }
}
