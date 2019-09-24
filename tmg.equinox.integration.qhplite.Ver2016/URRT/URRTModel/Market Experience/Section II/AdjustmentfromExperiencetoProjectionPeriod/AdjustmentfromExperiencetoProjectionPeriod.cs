using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    /// <summary>
    /// Encapsulates the AdjustmentfromExperiencetoProjectionPeriod data  in Section II: Allowed Claims, PMPM basis
    /// URRT Template V 2015
    /// </summary>
    public class AdjustmentfromExperiencetoProjectionPeriod
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public InpatientHosipitalAdjustmentfromExperiencetoProjectionPeriod InpatientHosipitalAdjustmentfromExperiencetoProjectionPeriod { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public OutpatientHosipitalAdjustmentfromExperiencetoProjectionPeriod OutpatientHosipitalAdjustmentfromExperiencetoProjectionPeriod { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public ProfessionalAdjustmentfromExperiencetoProjectionPeriod ProfessionalAdjustmentfromExperiencetoProjectionPeriod { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public OtherMedicalAdjustmentfromExperiencetoProjectionPeriod OtherMedicalAdjustmentfromExperiencetoProjectionPeriod { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public CapitationAdjustmentfromExperiencetoProjectionPeriod CapitationAdjustmentfromExperiencetoProjectionPeriod { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public PrescriptionDrugAdjustmentfromExperiencetoProjectionPeriod PrescriptionDrugAdjustmentfromExperiencetoProjectionPeriod { get; set; }
    }
}
