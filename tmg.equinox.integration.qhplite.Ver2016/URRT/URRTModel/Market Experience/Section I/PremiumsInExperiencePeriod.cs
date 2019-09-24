using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    /// <summary>
    /// Holds information for Premiums (net of MLR Rebate) in Experience Period:
    /// in Section I of URRT Template 2015
    /// </summary>
    public class PremiumsInExperiencePeriod
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "F", Row = 14, ParentName = "PremiumsInExperiencePeriod")]
        public string PeriodAggregate { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "G", Row = 14, ParentName = "PremiumsInExperiencePeriod")]
        public string PMPM { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "H", Row = 14, ParentName = "PremiumsInExperiencePeriod")]
        public string PercentageOfPremium { get; set; }
    }
}
