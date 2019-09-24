using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    public class OtherMedicalAdjustmentfromExperiencetoProjectionPeriod
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "J", Row = 27, ParentName = "OtherMedicalAdjustmentfromExperiencetoProjectionPeriod")]
        public string PopulationRiskMorbidity { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "K", Row = 27, ParentName = "OtherMedicalAdjustmentfromExperiencetoProjectionPeriod")]
        public string Other { get; set; }
    }
}
