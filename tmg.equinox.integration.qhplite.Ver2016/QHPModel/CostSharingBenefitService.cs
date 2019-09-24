using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{

    public class CostSharingBenefitService
    {
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "DJ", Row = 1)]
        public string ServiceName { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "DJ")]
        public string InNetworkCopay { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "DK")]
        public string InNetworkTier2Copay { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "DL")]
        public string OutOfNetworkCopay { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "DM")]
        public string InNetworkCoinsurance { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "DN")]
        public string InNetworkTier2Coinsurance { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "DO")]
        public string OutOfNetworkCoinsurance { get; set; }
    }
}
