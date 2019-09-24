using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnDeductible :Entity
    {
        public int DEDE_ID { get; set; }
        public decimal DEDE_AMT { get; set; }
        public string ABBREV { get; set; }
    }
}
