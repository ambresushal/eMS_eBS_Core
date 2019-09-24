using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    public class ProjectedExperiencePeriodInformation
    {
        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 79, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string PlanIDStandardComponentID { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 80, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string PlanAdjustedIndexRate { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 81, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string MemberMonths { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 82, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string TotalPremium { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 83, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string EHBPercentageOfTotalPremium { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 84, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string StateMandatedBenefitPortionPercentageOfTotalPremium { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 85, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string OtherBenefitPortionOfTotalPremium { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 86, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string TotalAllowedClaims { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 87, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string EHBPercentageOfTotalAllowedClaims { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 88, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string StateMandatedBenefitPortionPercentageOfTotalAllowedClaims { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 89, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string OtherBenefitPortionTotalAllowedClaims { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 90, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string NonIssuerObligationAllowedClaims { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 91, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string HHSFundPortionDollors { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 92, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string HHSFundPortionPercentage { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 93, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string TotalIncurredClaimsWithIssuerFunds { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 95, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string NetAmountOfReinsurance { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 96, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string NetAmountOfRiskAdjustment { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 98, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string IncurredClaimsPMPM { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 99, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string AllowedClaimsPMPM { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 100, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string EHBPortionOfAllowedClaimsPMPM { get; set; }

    }
}
