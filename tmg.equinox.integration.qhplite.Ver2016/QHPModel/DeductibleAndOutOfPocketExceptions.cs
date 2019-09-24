using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class DeductibleAndOutOfPocketExceptions
    {
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "M")]
        public string SubjectToDeductibleTier1 { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "N")]
        public string SubjectToDeductibleTier2 { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "O")]
        public string ExcludedFromInNetworkMOOP { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "P")]
        public string ExcludedFromOutOfNetworkMOOP { get; set; }
    }
}
