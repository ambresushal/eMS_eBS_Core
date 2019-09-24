using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class URRTPlanProductInfo
    {
        public List<SectionIGeneralProductandPlanInformation> SectionIGeneralProductandPlanInformation { get; set; }
        public SectionIIComponentsofPremiumIncreasePMPMDollarAmountaboveCurrentAverag SectionIIComponentsofPremiumIncreasePMPMDollarAmountaboveCurrentAverag { get; set; }
        public List<SectionIIIExperiencePeriodInformation> SectionIIIExperiencePeriodInformation { get; set; }
        public List<SectionIVProjected12monthsfollowingeffectivedate> SectionIVProjected12monthsfollowingeffectivedate { get; set; }
    }
}
