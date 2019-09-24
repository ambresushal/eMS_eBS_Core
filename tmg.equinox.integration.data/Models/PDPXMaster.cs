using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class PDPXMaster:Entity
    {
        public string PDBC_PFX { get; set; }
        public string PDBC_TYPE { get; set; }
        public string PDPX_DESC { get; set; }
        public string NEW_DESC { get; set; }
        public Int16? PDPX_LOCK_TOKEN { get; set; }
        public DateTime? ATXR_SOURCE_ID { get; set; }
    }
}
