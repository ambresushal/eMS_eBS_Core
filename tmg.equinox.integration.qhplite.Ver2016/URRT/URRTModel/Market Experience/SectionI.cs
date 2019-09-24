using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    /// <summary>
    /// Encapsulates the data in Section I : Experience Period Data in 
    /// URRT Template V 2015
    /// </summary>
    public class SectionI
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "E", Row = 12)]
        public string ExperiencePeriodFrom { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience,Column = "G", Row = 12)]
        public string ExperiencePeriodTo { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public PremiumsInExperiencePeriod PremiumsinExperiencePeriod { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public IncurredClaimsInExperiencePeriod IncurredClaimsInExperiencePeriod { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public AllowedClaim AllowedClaim { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public IndexRateofExperiencePeriod IndexRateofExperiencePeriod { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public ExperiencePeriodMemberMonths ExperiencePeriodMemberMonths { get; set; }

        public SectionI()
        {
            PremiumsinExperiencePeriod = new PremiumsInExperiencePeriod();
            IncurredClaimsInExperiencePeriod = new IncurredClaimsInExperiencePeriod();
            AllowedClaim = new AllowedClaim();
            IndexRateofExperiencePeriod = new IndexRateofExperiencePeriod();
            ExperiencePeriodMemberMonths = new ExperiencePeriodMemberMonths();
        }

    }
}
