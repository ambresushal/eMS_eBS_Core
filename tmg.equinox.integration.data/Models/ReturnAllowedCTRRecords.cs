using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnAllowedCTRRecords : Entity
    {
        public int ALLOW_CTR_DISTINCT_ID { get; set; }
        public Nullable<short> SETR_ALLOW_CTR { get; set; }
        public string ABBREV { get; set; }     
    }
}
