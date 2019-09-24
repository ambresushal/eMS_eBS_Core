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
    public class CredibilityManual
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public InpatientHospitalCredibilityManual InpatientHospitalCredibilityManual { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public OutpatientHospitalCredibilityManual OutpatientHospitalCredibilityManual { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public ProfessionalCredibilityManual ProfessionalCredibilityManual { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public OtherMedicalCredibilityManual OtherMedicalCredibilityManual { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public CapitationCredibilityManual CapitationCredibilityManual { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public PrescriptionDrugCredibilityManual PrescriptionDrugCredibilityManual { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "T", Row = 30, ParentName = "CredibilityManual")]
        public string Total { get; set; }
    }
}
