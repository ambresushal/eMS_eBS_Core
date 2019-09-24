using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class MaximumOutOfPocketForDrugEHBBenefits
    {
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "AF")]
        public string MaximumOutofPocketType { get; set; }        
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "AF")]
        public string InNetworkIndividual { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "AG")]
        public string InNetworkFamily { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "AH")]
        public string InNetworkTier2Individual { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "AI")]
        public string InNetworkTier2Family { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "AJ")]
        public string OutOfNetworkIndividual { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "AK")]
        public string OutOfNetworkFamily { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "AL")]
        public string CombinedInOutNetworkIndividual { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "AM")]
        public string CombinedInOutNetworkFamily { get; set; }
    }
}
