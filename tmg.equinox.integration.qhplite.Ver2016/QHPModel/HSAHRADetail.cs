using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class HSAHRADetail
    {
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "BZ")]
        public string HSAEligible { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "CA")]
        public string HSAHRAEmployerContribution { get; set; }
        [QHPSetting(SheetType = QHPSheetType.CostShareVariance, Column = "CB")]
        public string HSAHRAEmployerContributionAmount { get; set; }
    }
}
