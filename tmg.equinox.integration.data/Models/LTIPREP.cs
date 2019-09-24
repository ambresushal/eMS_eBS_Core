using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.data.Models
{
    public partial class LTIPREP:Entity
    {
        public int LTIPId { get; set; }
        public string LTLT_PFX { get; set; }
        public int ACAC_ACC_NO { get; set; }
        public string LTIP_IPCD_ID_LOW { get; set; }
        public string LTIP_IPCD_ID_HIGH { get; set; }
        public int LTIP_LOCK_TOKEN { get; set; }
        public System.DateTime ATXR_SOURCE_ID { get; set; }
        public string Hashcode { get; set; }
        
    }
}
