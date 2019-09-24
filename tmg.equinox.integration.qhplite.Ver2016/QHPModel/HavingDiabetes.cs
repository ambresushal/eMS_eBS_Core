using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class HavingDiabetes
    {
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "P")]
        public string Deductible { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "Q")]
        public string Copayment { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "R")]
        public string Coinsurance { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "S")]
        public string Limit { get; set; }
    }
}
