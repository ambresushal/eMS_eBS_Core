using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    public class ProfessionalAdjustmentfromExperiencetoProjectionPeriod
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "J", Row = 26, ParentName = "ProfessionalAdjustmentfromExperiencetoProjectionPeriod")]
        public string PopulationRiskMorbidity { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "K", Row = 26, ParentName = "ProfessionalAdjustmentfromExperiencetoProjectionPeriod")]
        public string Other { get; set; }
    }
}
