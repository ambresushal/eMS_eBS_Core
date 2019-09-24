using System.Collections.Generic;

namespace tmg.equinox.anocchart.Model
{
    public class BenefitsCompare
    {
        public string ANOCBenefitName { get; set; }
        public string BenefitCategory1 { get; set; }
        public string BenefitCategory2 { get; set; }
        public string BenefitCategory3 { get; set; }
        //public List<PlanYears> IQMedicareNetWorkList { get; set; }
        public string CostShareTiers { get; set; }
        public string NextYear { get; set; }
        public string ThisYear { get; set; }
        public string DisplayinANOC { get; set; }
        public int RowIDProperty { get; set; }
    }

}
