using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    /// <summary>
    /// Encapsulates the data in Section II: Allowed Claims, PMPM basis
    /// URRT Template V 2015
    /// </summary>
    public class SectionII
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public ExperiencePeriod ExperiencePeriod { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public OnActualExperienceAllowed OnActualExperienceAllowed { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public AdjustmentfromExperiencetoProjectionPeriod AdjustmentfromExperiencetoProjectionPeriod { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public AnnualizedTrendFactors AnnualizedTrendFactors { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public ProjectionsBeforeCredibilityAdjustment ProjectionsBeforeCredibilityAdjustment { get; set; }
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public CredibilityManual CredibilityManual { get; set; }
    }
}
