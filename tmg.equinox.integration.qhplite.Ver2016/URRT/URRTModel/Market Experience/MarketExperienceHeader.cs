using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    public class MarketExperienceHeader
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "E", Row = 3)]
        public string CompanyLegalName { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "H", Row = 3)]
        public string State { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "E", Row = 4)]
        public string HIOSIssuerID { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "H", Row = 4)]
        public string Market { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "E", Row = 5)]
        public string EffectiveDateofRateChanges { get; set; }
    }
}
