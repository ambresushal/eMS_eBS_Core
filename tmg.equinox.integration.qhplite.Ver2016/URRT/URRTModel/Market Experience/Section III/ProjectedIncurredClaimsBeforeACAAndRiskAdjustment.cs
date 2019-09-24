using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    /// <summary>
    /// Encapsulates the ProjectedIncurredClaimsBeforeACAAndRiskAdjustment data in Section III: Projected Experience
    /// URRT Template V 2015
    /// </summary>
    public class ProjectedIncurredClaimsBeforeACAAndRiskAdjustment
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "V", Row = 34)]
        public string AfterCredibility { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "X", Row = 34)]
        public string ProjectedPeriodTotals { get; set; }
    }
}
