using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    public class ComponentOfPremiumIncrease
    {
        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 32, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string PlanIDStandardComponentID { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 33, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string Inpatient { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 34, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string OutPatient { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 35, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string Professional { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 36, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string PrescriptionDrug { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 37, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string Other { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 38, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string Capitation { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 39, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string Administration { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 40, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string TaxesAndFees { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 41, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string RiskAndProfitCharge { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 42, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string TotalRateIncrease { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 43, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string MemberCostShareIncrease { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 46, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string AverageCurrentRatePMPM { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 47, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string ProjectedMemberMonths { get; set; }
    }
}
