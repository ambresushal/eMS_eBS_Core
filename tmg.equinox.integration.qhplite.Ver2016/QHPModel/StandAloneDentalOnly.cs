using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class StandAloneDentalOnly
    {
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "Y")]
        public string EHBApportionmentForPediatricDental { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "Z")]
        public string GuaranteedVsEstimatedRate { get; set; }
    }
}
