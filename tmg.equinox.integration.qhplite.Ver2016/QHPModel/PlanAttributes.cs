using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class PlanAttributes
    {
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "H")]
        public string NewExistingPlan { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "I")]
        public string PlanType { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "J")]
        public string LevelOfCoverage { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "K")]
        public string DesignType { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "L")]
        public string UniquePlanDesign { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "M")]
        public string QHPNonQHP { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "N")]
        public string NoticeRequiredForPregnancy { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "O")]
        public string IsAReferralRequiredForSpecialist { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "P")]
        public string SpecialistsRequiringAReferral { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "Q")]
        public string PlanLevelExclusions { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "R")]
        public string LimitedCostSharingPlanVariation { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "S")]
        public string DoesthisplanofferCompositeRating { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "T")]
        public string ChildOnlyOffering { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "U")]
        public string ChildOnlyPlanID { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "V")]
        public string TobaccoWellnessProgramOffered { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "W")]
        public string DiseaseManagementProgramsOffered { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Column = "X")]
        public string EHBPercentofTotalPremium { get; set; }
    }
}
