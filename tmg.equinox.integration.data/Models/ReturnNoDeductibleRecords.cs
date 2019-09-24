using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnNoDeductibleRecords : Entity
    {
        public int NODEDUCTIBLE_ID { get; set; }
        public string DEDUCTIBLE { get; set; }
        public string ABBREV { get; set; }
    }
}
