using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    /// <summary>
    /// Holds information of Allowed Claims
    /// in Section I of URRT Template 2015
    /// </summary>
    public class AllowedClaim
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, Column = "F", Row = 16, ParentName = "AllowedClaim")]
        public string PeriodAggregate { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience, Column = "G", Row = 16, ParentName = "AllowedClaim")]
        public string PMPM { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience, Column = "H", Row = 16, ParentName = "AllowedClaim")]
        public string PercentageOfPremium { get; set; }
    }
}
