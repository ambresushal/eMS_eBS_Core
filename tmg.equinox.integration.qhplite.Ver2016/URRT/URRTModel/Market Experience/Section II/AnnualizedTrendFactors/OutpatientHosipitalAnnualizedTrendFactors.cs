using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    public class OutpatientHosipitalAnnualizedTrendFactors
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "L", Row = 25, ParentName = "OutpatientHosipitalAnnualizedTrendFactors")]
        public string Cost { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "M", Row = 25, ParentName = "OutpatientHosipitalAnnualizedTrendFactors")]
        public string Utilization { get; set; }
    }
}
