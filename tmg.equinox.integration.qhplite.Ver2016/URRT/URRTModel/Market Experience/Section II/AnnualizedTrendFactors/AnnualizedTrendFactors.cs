using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    /// <summary>
    /// Encapsulates the AnnualizedTrendFactors data  in Section II: Allowed Claims, PMPM basis
    /// URRT Template V 2015
    /// </summary>
    public class AnnualizedTrendFactors
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public InpatientHosipitalAnnualizedTrendFactors InpatientHosipitalAnnualizedTrendFactors { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public OutpatientHosipitalAnnualizedTrendFactors OutpatientHosipitalAnnualizedTrendFactors { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public ProfessionalAnnualizedTrendFactors ProfessionalAnnualizedTrendFactors { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public OtherMedicalAnnualizedTrendFactors OtherMedicalAnnualizedTrendFactors { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public CapitationAnnualizedTrendFactors CapitationAnnualizedTrendFactors { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public PrescriptionDrugAnnualizedTrendFactors PrescriptionDrugAnnualizedTrendFactors { get; set; }
    }
}
