using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class PremiumInformation
    {
        public string PlanName { get; set; }
        public string PlanIDStandardComponentID { get; set; }
        public string AverageRatePMPM { get; set; }
        public string PlanAdjustedIndexRate { get; set; }
        public string MemberMonths { get; set; }
        public string TotalPremiumTP { get; set; }
        public string EHBPercentofTP { get; set; }
        public string StatemandatedbenefitsportionofTPthatareotherthanEHB { get; set; }
        public string OtherbenefitsportionofTP { get; set; }
    }
}
