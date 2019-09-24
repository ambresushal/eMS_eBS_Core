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
    public class ProjectionsBeforeCredibilityAdjustment
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public InpatientHospitalProjectionsBeforeCredibilityAdjustment InpatientHospitalProjectionsBeforeCredibilityAdjustment { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public OutpatientHospitalProjectionsBeforeCredibilityAdjustment OutpatientHospitalProjectionsBeforeCredibilityAdjustment { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public ProfessionalProjectionsBeforeCredibilityAdjustment ProfessionalProjectionsBeforeCredibilityAdjustment { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public OtherMedicalProjectionsBeforeCredibilityAdjustment OtherMedicalProjectionsBeforeCredibilityAdjustment { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public CapitationProjectionsBeforeCredibilityAdjustment CapitationProjectionsBeforeCredibilityAdjustment { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public PrescriptionDrugProjectionsBeforeCredibilityAdjustment PrescriptionDrugProjectionsBeforeCredibilityAdjustment { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "Q", Row = 30, ParentName = "ProjectionsBeforeCredibilityAdjustment")]
        public string Total { get; set; }
    }
}
