using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    /// <summary>
    /// Encapsulates the AdministrativeExpenseLoad data in Section III: Projected Experience
    /// URRT Template V 2015
    /// </summary>
    public class AdministrativeExpenseLoad
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "T", Row = 40)]
        public string PMPM { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "V", Row = 40)]
        public string AfterCredibility { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "X", Row = 40)]
        public string ProjectedPeriodTotals { get; set; }
    }
}
