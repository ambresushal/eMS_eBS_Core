using System.Collections.Generic;

namespace tmg.equinox.anocchart.Model
{
    public class CostShareTier
    {
        public string CoveredCosts { get; set; }
        //public List<PlanYears> IQMedicareNetWorkList { get; set; }
        public string DisplayinANOC { get; set; }
        public string ThisYear { get; set; }
        public string NextYear { get; set; }
        public int RowIDProperty { get; set; }

    }
    public class PlanYears
    {
        public string CostShareTiers { get; set; }
        public string ThisYear { get; set; }
        public string NextYear { get; set; }
    }
}
