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
    public class PrescriptionDrugCredibilityManual
    {
        //[URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "C", Row = 29, ParentName = "PrescriptionDrugCredibilityManual")]
        public string BenefitCatergory { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "R", Row = 29, ParentName = "PrescriptionDrugCredibilityManual")]
        public string OnPer1000 { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "S", Row = 29, ParentName = "PrescriptionDrugCredibilityManual")]
        public string CostPerService { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "T", Row = 29, ParentName = "PrescriptionDrugCredibilityManual")]
        public string PMPM { get; set; }
    }
}
