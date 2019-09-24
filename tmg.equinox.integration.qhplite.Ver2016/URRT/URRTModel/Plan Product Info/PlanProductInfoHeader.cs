using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    public class PlanProductInfoHeader
    {
        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "I", Row = 3)]
        public string CompanyLegalName { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "Q", Row = 3)]
        public string State { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "I", Row = 4)]
        public string HIOSIssuerID { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "Q", Row = 4)]
        public string Market { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, Column = "I", Row = 5)]
        public string EffectiveDateofRateChanges { get; set; }
    }
}
