using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    /// <summary>
    /// Encapsulates the ProjectedIncurredClaimsBeforeReinsuranceRecoveries data in Section III: Projected Experience
    /// URRT Template V 2015
    /// </summary>
    public class ProjectedIncurredClaimsBeforeReinsuranceRecoveries
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "V", Row = 36)]
        public string AfterCredibility { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "X", Row = 36)]
        public string ProjectedPeriodTotals { get; set; }
    }
}
