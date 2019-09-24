using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    public class ExperiencePeriodInformation
    {
        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 53, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string PlanIDStandardComponentID { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 54, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string AverageRatePMPM { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 55, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string MemberMonths { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 56, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string TotalPremium { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 57, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string EHBPercentageOfTotalPremium { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 58, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string StateMandatedBenefitPortionPercentageOfTotalPremium { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 59, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string OtherBenefitPortionOfTotalPremium { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 60, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string TotalAllowedClaims { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 61, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string EHBPercentageOfTotalAllowedClaims { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 62, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string StateMandatedBenefitPortionPercentageOfTotalAllowedClaims { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 63, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string OtherBenefitPortionTotalAllowedClaims { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 64, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string NonIssuerObligationAllowedClaims { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 65, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string HHSFundPortionDollors { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 66, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string HHSFundPortionPercentage { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 67, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string TotalIncurredClaimsWithIssuerFunds { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 69, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string NetAmountOfReinsurance { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 70, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string NetAmountOfRiskAdjustment { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 72, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string IncurredClaimsPMPM { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 73, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string AllowedClaimsPMPM { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 74, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string EHBPortionOfAllowedClaimsPMPM { get; set; }

    }
}
