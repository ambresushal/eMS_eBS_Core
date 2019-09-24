using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class PlanCostSharingAttributes
    {
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "A")]
        public string HIOSPlanIDComponentAndVariant { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "B")]
        public string PlanMarketingName { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "C")]
        public string LevelOfCoverage { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "D")]
        public string CSRVariationType { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "E")]
        public string IssuerActuarialValue { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "F")]
        public string AVCalculatorOutputNumber { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "G")]
        public string MedicalAndDrugDeductiblesIntegrated { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "H")]
        public string MedicalAndDrugOutOfPocketIntegrated { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "I")]
        public string MultipleInNetworkTiers { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "J")]
        public string FirstTierUtilization { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "K")]
        public string SecondTierUtilization { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, IsContainer = true, IsList = false)]
        public SBCScenario SBCScenario { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, IsContainer = true, IsList = false)]
        public MaximumOutOfPocketForMedicalEHBBenefits MaximumOutOfPocketForMedicalEHBBenefits { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, IsContainer = true, IsList = false)]
        public MaximumOutOfPocketForDrugEHBBenefits MaximumOutOfPocketForDrugEHBBenefits { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, IsContainer = true, IsList = false)]
        public MaximumOutOfPocketForMedicalAndDrugEHBBenefits MaximumOutOfPocketForMedicalAndDrugEHBBenefits { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, IsContainer = true, IsList = false)]
        public MedicalEHBDeductible MedicalEHBDeductible{ get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, IsContainer = true, IsList = false)]
        public DrugEHBDeductible DrugEHBDeductible { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, IsContainer = true, IsList = false)]
        public CombinedMedicalEHBDeductible CombinedMedicalEHBDeductible { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, IsContainer = true, IsList = false)]
        public HSAHRADetail HSAHRADetail { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, IsContainer = true, IsList = false)]
        public PlanVariantLevelURLs PlanVariantLevelURLs { get; set; }

        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, IsContainer = true, IsList = false)]
        public AVCalculatorAdditionalBenefitDesign AVCalculatorAdditionalBenefitDesign { get; set; }

        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "CI", Row = 1, IncrementDirection = IncrementDirection.Column, IncrementStep = 8, IsContainer = true, IsList = true)]
        public List<DeductibleSubGroup> DeductibleSubGroups { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "DJ", Row = 1, IncrementDirection = IncrementDirection.Column, IncrementStep = 6, IsContainer = true, IsList = true)]
        public List<CostSharingBenefitService> CostSharingBenefitServices { get; set; }
        public string HavingDiabetesCoinsurance { get; set; }
        public string HavingDiabetesCopayment { get; set; }
        public string HavingDiabetesDeductible { get; set; }
        public string HavingDiabetesLimit { get; set; }
    }
}
