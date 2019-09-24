using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    /// <summary>
    /// Holds information of Experience Period Member Months
    /// in Section I of URRT Template 2015
    /// </summary>
    public class ExperiencePeriodMemberMonths
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "F", Row = 18, ParentName = "ExperiencePeriodMemberMonths")]
        public string PeriodAggregate { get; set; }
    }
}
