using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class ClaimsInformation
    {
        public string PlanName { get; set; }
        public string PlanIDStandardComponentID { get; set; }
        public string TotalAllowedClaimsTAC { get; set; }
        public string EHBPercentofTAC { get; set; }
        public string StatemandatedbenefitsportionofTACthatareotherthanEHB { get; set; }
        public string OtherbenefitsportionofTAC { get; set; }
        public string AllowedClaimswhicharenottheissuersobligation { get; set; }
        public string PortionoftheabovepayablebyHHSsfundsonbehalfofinsuredpersonsindollars { get; set; }
        public string PortionofabovepayablebyHHSonbehalfofinsuredpersonasPercentage { get; set; }
        public string TotalIncurredclaimspayablewithissuerfunds { get; set; }
        public string NetAmtofRein { get; set; }
        public string NetAmtofRiskAdj { get; set; }
        public string IncurredClaimsPMPM { get; set; }
        public string AllowedClaimsPMPM { get; set; }
        public string EHBportionofAllowedClaimsPMPM { get; set; }
    }
}
