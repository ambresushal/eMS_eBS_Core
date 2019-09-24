using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class HavingATreatment
    {
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "T")]
        public string Deductible { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "U")]
        public string Copayment { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "V")]
        public string Coinsurance { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "W")]
        public string Limit { get; set; }
    }
}
