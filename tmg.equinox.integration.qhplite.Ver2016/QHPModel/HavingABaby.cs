using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class HavingABaby
    {
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "L")]
        public string Deductible { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "M")]
        public string Copayment { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "N")]
        public string Coinsurance { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "O")]
        public string Limit { get; set; }
    }
}
