using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class MaximumOutOfPocketForMedicalEHBBenefits
    {
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "X")]
        public string MaximumOutofPocketType { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "X")]
        public string InNetworkIndividual { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "Y")]
        public string InNetworkFamily { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "Z")]
        public string InNetworkTier2Individual { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "AA")]
        public string InNetworkTier2Family { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "AB")]
        public string OutOfNetworkIndividual { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "AC")]
        public string OutOfNetworkFamily { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "AD")]
        public string CombinedInOutNetworkIndividual { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "AE")]
        public string CombinedInOutNetworkFamily { get; set; }
    }
}
