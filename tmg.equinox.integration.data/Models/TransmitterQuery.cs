using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public class TransmitterQuery : Entity
    {
        public int TransmitterQuery1Up { get; set; }
        public int ProcessGovernance1Up { get; set; }
        public int? TranslatorGovernance1Up { get; set; }
        public string ProductID { get; set; }
        public string QueryType { get; set; }
        public string Component { get; set; }
        public string SqlQuery { get; set; }
        public int ProductSequenceNo { get; set; }                
    }
}
