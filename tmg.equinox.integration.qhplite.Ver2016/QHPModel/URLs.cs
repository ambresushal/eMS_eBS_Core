using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class URLs
    {
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "AL")]
        public string URLForSummaryOfBenefitsAndCoverage { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "AM")]
        public string URLForEnrollmentPayment { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "AN")]
        public string PlanBrochure { get; set; }
    }
}
