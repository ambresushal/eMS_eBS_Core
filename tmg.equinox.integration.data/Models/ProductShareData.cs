using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.data.Models
{
    public class ProductShareData : Entity
    {
        public string GRGR_NAME { get; set; }
        public string GRGR_ID { get; set; }
        public string PDPD_ID { get; set; }
        public System.DateTime GRGR_TERM_DT { get; set; }
        public string PDBC_PFX { get; set; }
        public string SEPY_PFX { get; set; }
        public string DEDE_PFX { get; set; }
        public string LTLT_PFX { get; set; }
    }
}
