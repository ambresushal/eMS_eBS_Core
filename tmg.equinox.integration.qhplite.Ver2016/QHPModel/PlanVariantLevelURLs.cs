using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class PlanVariantLevelURLs
    {
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "CC")]
        public string URLforSummaryofBenefitsCoverage { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "CD")]
        public string PlanBrochure { get; set; }
    }
}
