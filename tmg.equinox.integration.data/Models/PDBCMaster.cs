using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.data.Models
{
    public partial class PDBCMaster:Entity
    {
        //public int PDBCId { get; set; }
        public string PDBC_PFX { get; set; }
        public string PDPD_ID { get; set; }
        public string PDBC_TYPE { get; set; }
        public System.DateTime? PDBC_EFF_DT { get; set; }
        public System.DateTime? PDBC_TERM_DT { get; set; }
        public string PDBC_OPTS { get; set; }
        public Int16? PDBC_LOCK_TOKEN { get; set; }
        public System.DateTime? ATXR_SOURCE_ID { get; set; }
        public Nullable<System.DateTime> SYS_LAST_UPD_DTM { get; set; }
        public string SYS_USUS_ID { get; set; }
        public string SYS_DBUSER_ID { get; set; }
        public string Hashcode { get; set; }       
    }
}
