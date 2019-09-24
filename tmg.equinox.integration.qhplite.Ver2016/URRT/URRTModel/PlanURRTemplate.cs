using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    public class PlanURRTemplate
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public MarketExperience MarketExperience { get; set; }
        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, IsContainer = true)]
        public PlanProductInfo PlanProductInfo { get; set; }
    }
}
