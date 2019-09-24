using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class PrefixDescriptionLog : Entity
    {
        public int PrefixDescriptionLogID { get; set; }
        public string PDBC_PFX { get; set; }
        public string PDBC_TYPE { get; set; }
        public string ProductID { get; set; }
        public string Old_Desc { get; set; }
        public string New_Desc { get; set; } 
        public bool IsTransmitted { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
