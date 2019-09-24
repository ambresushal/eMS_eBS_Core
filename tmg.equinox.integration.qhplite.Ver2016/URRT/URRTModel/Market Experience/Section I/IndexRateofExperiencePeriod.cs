using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    /// <summary>
    /// Holds information of Allowed Claims
    /// in Section I of URRT Template 2015
    /// </summary>
    public class IndexRateofExperiencePeriod
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "G", Row = 17, ParentName = "IndexRateofExperiencePeriod")]
        public string PMPM { get; set; }
    }
}
