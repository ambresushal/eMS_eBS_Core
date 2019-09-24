using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class GeographicCoverage
    {
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "AC")]
        public string OutOfCountryCoverage { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "AD")]
        public string OutOfCountryCoverageDescription { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "AE")]
        public string OutOfServiceAreaCoverage { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "AF")]
        public string OutOfServiceAreaCoverageDescription { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "AG")]
        public string NationalNetwork { get; set; }
    }
}
