using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    /// <summary>
    /// Encapsulates the data in Section III: Projected Experience
    /// URRT Template V 2015
    /// </summary>
    public class SectionIII
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public ProjectedAllowedExperienceClaimsPMPM ProjectedAllowedExperienceClaimsPMPM { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public PaidtoAllowedAverageFactorinProjectionPeriod PaidtoAllowedAverageFactorinProjectionPeriod { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public ProjectedIncurredClaimsBeforeACAAndRiskAdjustment ProjectedIncurredClaimsBeforeACAAndRiskAdjustment { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public ProjectedRiskAdjustmentsPMPM ProjectedRiskAdjustmentsPMPM { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public ProjectedIncurredClaimsBeforeReinsuranceRecoveries ProjectedIncurredClaimsBeforeReinsuranceRecoveries { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public ProjectedACAReinsuranceRecoveries ProjectedACAReinsuranceRecoveries { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public ProjectedIncurredClaims ProjectedIncurredClaims { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public AdministrativeExpenseLoad AdministrativeExpenseLoad { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public ProfitAndRiskLoad ProfitAndRiskLoad { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public TaxesAndFees TaxesAndFees { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public SingleRiskPoolGrossPremiumAverageRate SingleRiskPoolGrossPremiumAverageRate { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public IndexRateforProjectionPeriod IndexRateforProjectionPeriod { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience, Column = "X", Row = 47)]
        public string ProjectedMemberMonths { get; set; }
    }
}
