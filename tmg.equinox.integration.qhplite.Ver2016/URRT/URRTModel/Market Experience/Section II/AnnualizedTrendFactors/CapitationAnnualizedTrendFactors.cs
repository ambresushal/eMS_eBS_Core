using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    public class CapitationAnnualizedTrendFactors
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "L", Row = 28, ParentName = "CapitationAnnualizedTrendFactors")]
        public string Cost { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "M", Row = 28, ParentName = "CapitationAnnualizedTrendFactors")]
        public string Utilization { get; set; }
    }
}
