using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class PlanBenefitPackage
    {
        public string PackageName { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage , Row = 2, Column = "B", IncrementDirection = IncrementDirection.None, IncrementStep = 0)]
        public string HIOSIssuerID { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Row = 3, Column = "B", IncrementDirection = IncrementDirection.None, IncrementStep = 0)]
        public string IssuerState { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Row = 4, Column = "B", IncrementDirection = IncrementDirection.None, IncrementStep = 0)]
        public string MarketCoverage { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Row = 5, Column = "B", IncrementDirection = IncrementDirection.None, IncrementStep = 0)]
        public string DentalOnlyPlan { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Row = 6, Column = "B", IncrementDirection = IncrementDirection.None, IncrementStep = 0)]
        public string TIN { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Row = 9, Column="A", IncrementDirection = IncrementDirection.Row, IncrementStep = 1, IsContainer = true, IsList = true)]
        public List<PlanIdentifier> PlanIdentifiers { get; set; }
        [QHPSetting(SheetType = QHPSheetType.BenefitPackage, Row = 61, Column = "A", IncrementDirection = IncrementDirection.Row, IncrementStep = 1, IsContainer = true, IsList = true)]
        public List<Benefit> Benefits { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Row = 4, Column = "A", IncrementDirection = IncrementDirection.Row, IncrementStep = 1, IsContainer = true, IsList = true)]
        public List<PlanCostSharingAttributes> PlanCostSharingAttributes { get; set; }
        public List<SectionIGeneralProductandPlanInformation> SectionIGeneralProductandPlanInformation { get; set; }
        public List<SectionIIComponentsofPremiumIncreasePMPMDollarAmountaboveCurrentAverag> SectionIIComponentsofPremiumIncreasePMPMDollarAmountaboveCurrentAverag { get; set; }
        public List<SectionIIIExperiencePeriodInformation> SectionIIIExperiencePeriodInformation { get; set; }
        public List<SectionIVProjected12monthsfollowingeffectivedate> SectionIVProjected12monthsfollowingeffectivedate { get; set; }
   
    }
}
