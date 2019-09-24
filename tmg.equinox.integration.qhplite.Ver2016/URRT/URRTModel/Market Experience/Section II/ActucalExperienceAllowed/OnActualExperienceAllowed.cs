using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    /// <summary>
    /// Encapsulates the data in on Actual Experience Allowed in Section II - Allowed Claims, PMPM basis
    /// URRT Template V 2015
    /// </summary>
    public class OnActualExperienceAllowed
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public InpatientHospitalActualExperienceAllowed InpatientHospitalActualExperienceAllowed { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public OutpatientHospitalActualExperienceAllowed OutpatientHospitalActualExperienceAllowed { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public ProfessionalActualExperienceAllowed ProfessionalActualExperienceAllowed { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public OtherMedicalActualExperienceAllowed OtherMedicalActualExperienceAllowed { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public CapitationActualExperienceAllowed CapitationActualExperienceAllowed { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public PrescriptionDrugActualExperienceAllowed PrescriptionDrugActualExperienceAllowed { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience, Column = "H", Row = 30, ParentName = "OnActualExperienceAllowed")]
        public string Total { get; set; }
    }
}
