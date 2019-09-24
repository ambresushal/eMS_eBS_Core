using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.sbccalculator.Model
{
    public class CostShare
    {
        public string NetworkName { get; set; }
        public string Copay { get; set; }
        public string Coinsurance { get; set; }
        public string Deductible { get; set; }
    }

    public class NetworkList
    {
        public string NetworkName { get; set; }
        public string NetworkTier { get; set; }
    }

    public class DeductibleList
    {
        public string CoverageName { get; set; }
        public string DeductibleAmount { get; set; }
        public string NetworkName { get; set; }
        public string NetworkTier { get; set; }
    }

    public class RxCostShare
    {
        public string Network { get; set; }
        public string RxDeductibleIndividual { get; set; }
    }
}
