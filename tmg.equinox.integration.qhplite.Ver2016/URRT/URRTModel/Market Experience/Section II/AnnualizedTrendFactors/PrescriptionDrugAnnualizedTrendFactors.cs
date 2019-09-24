using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    public class PrescriptionDrugAnnualizedTrendFactors
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "L", Row = 29, ParentName = "PrescriptionDrugAnnualizedTrendFactors")]
        public string Cost { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "M", Row = 29, ParentName = "PrescriptionDrugAnnualizedTrendFactors")]
        public string Utilization { get; set; }
    }
}
