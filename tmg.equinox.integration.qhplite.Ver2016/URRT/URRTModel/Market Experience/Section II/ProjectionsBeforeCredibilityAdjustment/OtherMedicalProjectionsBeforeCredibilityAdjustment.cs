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
    public class OtherMedicalProjectionsBeforeCredibilityAdjustment
    {
       // [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "C", Row = 27, ParentName = "OtherMedicalProjectionsBeforeCredibilityAdjustment")]
        public string BenefitCatergory { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "O", Row = 27, ParentName = "OtherMedicalProjectionsBeforeCredibilityAdjustment")]
        public string UtilizationPer1000 { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "P", Row = 27, ParentName = "OtherMedicalProjectionsBeforeCredibilityAdjustment")]
        public string CostPerService { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "Q", Row = 27, ParentName = "OtherMedicalProjectionsBeforeCredibilityAdjustment")]
        public string PMPM { get; set; }
    }
}
