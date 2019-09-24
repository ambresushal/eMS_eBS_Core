using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class PlanIdentifier
    {
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "A")]
        public string HIOSPlanID { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "B")]
        public string PlanMarketingName { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "C")]
        public string HIOSProductID { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "D")]
        public string HPID { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "E")]
        public string NetworkID { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "F")]
        public string ServiceAreaID { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "G")]
        public string FormularyID { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, IsContainer = true, IsList = false)]
        public PlanAttributes PlanAttributes { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, IsContainer = true, IsList = false)]
        public StandAloneDentalOnly StandAloneDentalOnly { get; set; }
        //[QHPSetting(SheetType = QHPSheetType.BenefitPackage, IsContainer = true, IsList = false)]
        //public AVCalculatorAdditionalBenefitDesign AVCalculatorAdditionalBenefitDesign { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, IsContainer = true, IsList = false)]
        public PlanDates PlanDates { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, IsContainer = true, IsList = false)]
        public GeographicCoverage GeographicCoverage { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, IsContainer = true, IsList = false)]
        public PlanLevelURLs PlanLevelURLs { get; set; }
    }
}
