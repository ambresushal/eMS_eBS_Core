using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnNoCoInsuranceRecords:Entity
    {
        public int NOCOINSURANCE_ID { get; set; }
        public string COIN_PCT { get; set; }
        public string ABBREV { get; set; }
    }
}
