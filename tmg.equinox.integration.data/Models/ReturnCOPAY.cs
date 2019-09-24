using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnCOPAY : Entity
    {
        public int COPAY_ID { get; set; }
        public decimal COPAY_AMT { get; set; }
        public string ABBREV { get; set; }
    }
}
