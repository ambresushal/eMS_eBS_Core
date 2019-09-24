using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class BenefitInformation
    {
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "A")]
        public string Benefit { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "C")]
        public string EHB { get; set; }
        //[QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "D")]
        //public string StateRequiredBenefit { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, IsContainer = true, IsList = false)]
        public GeneralInformation GeneralInformation { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, IsContainer = true, IsList = false)]
        public OutOfPocketExceptions DeductibleAndOutOfPocketExceptions { get; set; }
    }
}
