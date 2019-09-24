using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class SectionIGeneralProductandPlanInformation
    {
        public string Product { get; set; }
        public string ProductID { get; set; }
        public string Metal { get; set; }
        public string PlanType { get; set; }
        public string PlanName { get; set; }
        public string PlanIDStandardComponentID { get; set; }
        public string AVMetalValue { get; set; }
        public string AVPricingValue { get; set; }
        public string ExchangePlan { get; set; }
        public string HistoricalRateIncreaseCalendarYear2 { get; set; }
        public string HistoricalRateIncreaseCalendarYear1 { get; set; }
        public string HistoricalRateIncreaseCalendarYear0 { get; set; }
        public string EffectiveDateofProposedRates { get; set; }
        public string RateChangePercentoverpreviousfiling { get; set; }
        public string CumulativeRateChangePercentover12mosprior { get; set; }
        public string ProjectedPerRateChangePercentageoverExperiencePeriod { get; set; }
        public string ProductThresholdRateIncreasePercentage { get; set; }
    }
}
