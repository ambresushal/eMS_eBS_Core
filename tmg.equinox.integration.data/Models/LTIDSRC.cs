using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.data.Models
{
    public partial class LTIDSRC:Entity
    {
        public int LTIDId { get; set; }
        public string LTLT_PFX { get; set; }
        public Int16? ACAC_ACC_NO { get; set; }
        public string IDCD_ID_REL { get; set; }
        public Int16? LTID_LOCK_TOKEN { get; set; }
        public Nullable<System.DateTime> ATXR_SOURCE_ID { get; set; }
        public Nullable<System.DateTime> SYS_LAST_UPD_DTM { get; set; }
        public string SYS_USUS_ID { get; set; }
        public string SYS_DBUSER_ID { get; set; }
        public string IDCD_TYPE { get; set; }
        public int? ProcessGovernance1up { get; set; }        
    }
}
