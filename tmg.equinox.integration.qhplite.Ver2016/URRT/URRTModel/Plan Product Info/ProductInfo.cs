using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    public class ProductInfo
    {
        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, IsContainer = true)]
        public GeneralPlanAndProductInformation GeneralPlanAndProductInformation { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, IsContainer = true)]
        public ComponentOfPremiumIncrease ComponentOfPremiumIncrease { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, IsContainer = true)]
        public ExperiencePeriodInformation ExperiencePeriodInformation { get; set; }

        [URRTSetting(SheetType = URRTSheetType.PlanProductInfo, IsContainer = true)]
        public ProjectedExperiencePeriodInformation ProjectedExperiencePeriodInformation { get; set; }
    }
}
