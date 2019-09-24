using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    /// <summary>
    /// Encapsulates the data in Experience Period in Section II - Allowed Claims, PMPM basis
    /// URRT Template V 2015
    /// </summary>
    public class ExperiencePeriod
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "L", Row = 21, ParentName = "ExperiencePeriod")]
        public string ProjectPeriodFrom { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "O", Row = 21, ParentName = "ExperiencePeriod")]
        public string ProjectPeriodTo { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "T", Row = 21, ParentName = "ExperiencePeriod")]
        public string MidPointtoMidPointExperiencetoProjection { get; set; }
    }
}
