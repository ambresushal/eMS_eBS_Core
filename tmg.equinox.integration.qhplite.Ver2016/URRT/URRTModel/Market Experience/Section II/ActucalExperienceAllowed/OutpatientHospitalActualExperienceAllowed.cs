using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    /// <summary>
    /// Encapsulates the data for Benefit category on Actual Experience Allowed in Section II - Allowed Claims, PMPM basis
    /// URRT Template V 2015
    /// </summary>
    public class OutpatientHospitalActualExperienceAllowed
    {
       // [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "C", Row = 25, ParentName = "OutpatientHospitalActualExperienceAllowed")]
        public string BenefitCatergory { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "E", Row = 25, ParentName = "OutpatientHospitalActualExperienceAllowed")]
        public string UtilizationDescription { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "F", Row = 25, ParentName = "OutpatientHospitalActualExperienceAllowed")]
        public string UtilizationPer1000 { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "G", Row = 25, ParentName = "OutpatientHospitalActualExperienceAllowed")]
        public string CostPerService { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "H", Row = 25, ParentName = "OutpatientHospitalActualExperienceAllowed")]
        public string PMPM { get; set; }
    }
}
