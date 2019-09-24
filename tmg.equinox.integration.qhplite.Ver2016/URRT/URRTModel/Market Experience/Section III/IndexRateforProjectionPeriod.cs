using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    /// <summary>
    /// Encapsulates the IndexRateforProjectionPeriod data in Section III: Projected Experience
    /// URRT Template V 2015
    /// </summary>
    public class IndexRateforProjectionPeriod
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "V", Row = 44)]
        public string AfterCredibility { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "V", Row = 45)]
        public string PercentageIncreaseOverExperiencePeriodAfterCredibility { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "V", Row = 46)]
        public string PercentageIncreaseAnnualized { get; set; }
    }
}
