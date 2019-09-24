using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    public class CapitationAdjustmentfromExperiencetoProjectionPeriod
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "J", Row = 28, ParentName = "CapitationAdjustmentfromExperiencetoProjectionPeriod")]
        public string PopulationRiskMorbidity { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "K", Row = 28, ParentName = "CapitationAdjustmentfromExperiencetoProjectionPeriod")]
        public string Other { get; set; }
    }
}
