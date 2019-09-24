using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    public class OtherMedicalAnnualizedTrendFactors
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "L", Row = 27, ParentName = "OtherMedicalAnnualizedTrendFactors")]
        public string Cost { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "M", Row = 27, ParentName = "OtherMedicalAnnualizedTrendFactors")]
        public string Utilization { get; set; }
    }
}
