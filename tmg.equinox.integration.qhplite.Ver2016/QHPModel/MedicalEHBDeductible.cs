using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class MedicalEHBDeductible
    {
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "AV")]
        public string InNetworkIndividual { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "AW")]
        public string InNetworkFamily { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "AX")]
        public string InNetworkDefaultCoinsurance { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "AY")]
        public string InNetworkTier2Individual { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "AZ")]
        public string InNetworkTier2Family { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "BA")]
        public string InNetworkTier2DefaultCoinsurance { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "BB")]
        public string OutOfNetworkIndividual { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "BC")]
        public string OutOfNetworkFamily { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "BD")]
        public string CombinedInOutNetworkIndividual { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "BE")]
        public string CombinedInOutNetworkFamily { get; set; }
    }
}
