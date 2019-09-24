using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnCOInsurance:Entity
    {
        public int COINSURANCE_ID { get; set; }
        public decimal COIN_PCT { get; set; }
        public string ABBREV { get; set; }
    }
}
