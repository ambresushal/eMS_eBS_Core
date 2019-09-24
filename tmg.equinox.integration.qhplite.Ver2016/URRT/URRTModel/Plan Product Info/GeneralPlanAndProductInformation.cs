using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    public class GeneralPlanAndProductInformation
    {
        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 12, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string ProductName { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 13, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string ProductID { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 14, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string Metal { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 15, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string AVMetalValue { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 16, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string AVPricingValue { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 17, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string PlanType { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 18, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string PlanName { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 19, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string PlanIDStandardComponentID { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 20, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string ExchangePlan { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 21, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string HistoricalRateRelaeseCalendarYear2 { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 22, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string HistoricalRateReleaseCalendarYear1 { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 23, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string HistoricalRateReleaseCalendarYear0 { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 24, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string EffectiveDateOfProposedRates { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 25, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string RateChangePercentage { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 26, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string CummulativeRateChangePercentage { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 27, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string ProjectedPerRateChangePercentage { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "G", Row = 28, IncrementDirection = IncrementDirection.Column, IncrementStep = 1)]
        public string ProductThresholdRateIncreasePercentage { get; set; }
    }
}
