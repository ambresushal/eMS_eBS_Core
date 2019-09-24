using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    public class InpatientHosipitalAdjustmentfromExperiencetoProjectionPeriod
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "J", Row = 24, ParentName = "InpatientHosipitalAdjustmentfromExperiencetoProjectionPeriod")]
        public string PopulationRiskMorbidity { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "K", Row = 24, ParentName = "InpatientHosipitalAdjustmentfromExperiencetoProjectionPeriod")]
        public string Other { get; set; }
    }
}
