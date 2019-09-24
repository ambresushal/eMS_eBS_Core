using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class GeneralInformation
    {
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "D")]
        public string IsThisBenefitCovered { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "E")]
        public string QuantativeLimitOfService { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "F")]
        public string LimitQuantity { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "G")]
        public string LimitUnit { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "H")]
        //public string MinimumStay { get; set; }
        //[QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "I")]
        public string Exclusions { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "I")]
        public string BenefitExplanation { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "J")]
        public string EHBVarianceReason { get; set; }

    }
}
