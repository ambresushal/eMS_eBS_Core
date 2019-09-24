using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    public class PrescriptionDrugAdjustmentfromExperiencetoProjectionPeriod
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "J", Row = 29, ParentName = "PrescriptionDrugAdjustmentfromExperiencetoProjectionPeriod")]
        public string PopulationRiskMorbidity { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "K", Row = 29, ParentName = "PrescriptionDrugAdjustmentfromExperiencetoProjectionPeriod")]
        public string Other { get; set; }
    }
}
