using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    /// <summary>
    /// Encapsulates the ProjectedAllowedExperienceClaimsPMPM data in Section III: Projected Experience
    /// URRT Template V 2015
    /// </summary>
    public class ProjectedAllowedExperienceClaimsPMPM
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "Q", Row = 32)]
        public string BeforeCredibilityPMPM { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "T", Row = 32)]
        public string AfterCredibilityPMPM { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "V", Row = 32)]
        public string AfterCredibility { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "X", Row = 32)]
        public string ProjectedPeriodTotals { get; set; }
    }
}
