using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class CombinedMedicalEHBDeductible
    {
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "BP")]
        public string InNetworkIndividual { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "BQ")]
        public string InNetworkFamily { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "BR")]
        public string InNetworkDefaultCoinsurance { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "BS")]
        public string InNetworkTier2Individual { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "BT")]
        public string InNetworkTier2Family { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "BU")]
        public string InNetworkTier2DefaultCoinsurance { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "BV")]
        public string OutOfNetworkIndividual { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "BW")]
        public string OutOfNetworkFamily { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "BX")]
        public string CombinedInOutNetworkIndividual { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "BY")]
        public string CombinedInOutNetworkFamily { get; set; }
    }
}
