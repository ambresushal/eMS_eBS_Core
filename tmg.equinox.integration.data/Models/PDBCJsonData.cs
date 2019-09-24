using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class PDBCJsonData : Entity
    {
        public string ProductID { get; set; }
        public string PDPD_ID { get; set; }
        public string PDBC_TYPE { get; set; }
        public DateTime PDBC_EFF_DT  { get; set; }
        public DateTime PDBC_TERM_DT  { get; set; }
        public string PDBC_PFX { get; set; }
        public string PDBC_OPTS  { get; set; }
        public string PDBC_LOCK_TOKEN  { get; set; }
        public DateTime ATXR_SOURCE_ID  { get; set; }
        public DateTime SYS_LAST_UPD_DTM  { get; set; }
        public string SYS_USUS_ID { get; set; }
        public string SYS_DBUSER_ID { get; set; }
    }
}
