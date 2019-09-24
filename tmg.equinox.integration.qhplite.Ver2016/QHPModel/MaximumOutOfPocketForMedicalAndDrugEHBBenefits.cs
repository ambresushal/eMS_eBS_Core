using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class MaximumOutOfPocketForMedicalAndDrugEHBBenefits
    {       
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "AN")]
        public string InNetworkIndividual { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "AO")]
        public string InNetworkFamily { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "AP")]
        public string InNetworkTier2Individual { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "AQ")]
        public string InNetworkTier2Family { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "AR")]
        public string OutOfNetworkIndividual { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "AS")]
        public string OutOfNetworkFamily { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "AT")]
        public string CombinedInOutNetworkIndividual { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "AU")]
        public string CombinedInOutNetworkFamily { get; set; }
    }
}
