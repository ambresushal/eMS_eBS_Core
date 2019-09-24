using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    public class MarketExperience
    {
        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public MarketExperienceHeader Header { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public SectionI SectionI { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public SectionII SectionII { get; set; }

        [URRTSetting(SheetType = URRTSheetType.MarketExperience, IsContainer = true)]
        public SectionIII SectionIII { get; set; }

        public MarketExperience()
        {
            Header = new MarketExperienceHeader();
            SectionI = new SectionI();
        }
    }
}
