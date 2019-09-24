using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class AVCalculatorAdditionalBenefitDesign
    {
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "CE")]
        public string MaximumCoinsuranceForSpecialityDrugs { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "CF")]
        public string MaximumNumberOfDaysForChargingInpatientCopay { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "CG")]
        public string BeginPrimaryCostSharingAfterSetNumberOfVisits { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "CH")]
        public string BeginPrimaryCareDedCoAfterSetNumberOfCopays { get; set; }
    }
}
