using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.data.Models
{
    public partial class CBCNamingConventions : Entity
    {
        public int NamingConvention1up { get; set; }
        public int GroupID { get; set; }
        public string LOBD_ID { get; set; }
        public string BenefitSetName { get; set; }
        public Int16 PDVC_TIER { get; set; }
        public Int16 PDVC_SEQ_NO { get; set; }
    }
}
