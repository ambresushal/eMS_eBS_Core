using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    /// <summary>
    /// Holds information of Incurred Claims in Experience Period 
    /// in Section I of URRT Template 2015
    /// </summary>
    public class IncurredClaimsInExperiencePeriod
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "F", Row = 15, ParentName = "IncurredClaimsInExperiencePeriod")]
        public string PeriodAggregate { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "G", Row = 15, ParentName = "IncurredClaimsInExperiencePeriod")]
        public string PMPM { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "H", Row = 15, ParentName = "IncurredClaimsInExperiencePeriod")]
        public string PercentageOfPremium { get; set; }
    }
}
