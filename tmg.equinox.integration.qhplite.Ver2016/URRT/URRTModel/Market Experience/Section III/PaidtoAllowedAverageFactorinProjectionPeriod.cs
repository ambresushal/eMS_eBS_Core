using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    /// <summary>
    /// Encapsulates the PaidtoAllowedAverageFactorinProjectionPeriod data in Section III: Projected Experience
    /// URRT Template V 2015
    /// </summary>
    public class PaidtoAllowedAverageFactorinProjectionPeriod
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "V", Row = 33)]
        public string AfterCredibility { get; set; }
    }
}
