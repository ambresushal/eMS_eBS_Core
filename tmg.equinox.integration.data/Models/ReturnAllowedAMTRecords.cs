using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnAllowedAMTRecords : Entity
    {
        public int ALLOW_AMT_DISTINCT_ID { get; set; }
        public Nullable<decimal> SETR_ALLOW_AMT { get; set; }
        public string ABBREV { get; set; }        
    }
}
