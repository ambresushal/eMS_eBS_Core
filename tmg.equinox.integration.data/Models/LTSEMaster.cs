using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.data.Models
{
    public partial class LTSEMaster:Entity
    {
        public int LTSEId { get; set; }
        public string LTLT_PFX { get; set; }
        public Int16? ACAC_ACC_NO { get; set; }
        public string SESE_ID { get; set; }
        public int? LTSE_WT_CTR { get; set; }
        public Int16? LTSE_LOCK_TOKEN { get; set; }
        public System.DateTime? ATXR_SOURCE_ID { get; set; }
        public Nullable<System.DateTime> SYS_LAST_UPD_DTM { get; set; }
        public string SYS_USUS_ID { get; set; }
        public string SYS_DBUSER_ID { get; set; }
        public string HashCode { get; set; }
        
    }
}
