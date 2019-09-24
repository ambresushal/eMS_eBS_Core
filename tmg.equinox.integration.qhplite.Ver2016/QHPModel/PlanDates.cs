using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class PlanDates
    {
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "AA")]
        public string PlanEffectiveDate { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "AB")]
        public string PlanExpirationDate { get; set; }
    }
}
