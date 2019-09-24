using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.integration.qhplite.Ver2016.QHPModel
{
    public class SectionIIComponentsofPremiumIncreasePMPMDollarAmountaboveCurrentAverag
    {
        public string PlanName { get; set; }
        public string PlanIDStandardComponentID { get; set; }
        public string Inpatient { get; set; }
        public string Outpatient { get; set; }
        public string Professional { get; set; }
        public string PrescriptionDrug { get; set; }
        public string Other { get; set; }
        public string Capitation { get; set; }
        public string Administration { get; set; }
        public string TaxesFees { get; set; }
        public string RiskProfitCharge { get; set; }
        public string TotalRateIncrease { get; set; }
        public string MemberCostShareIncrease { get; set; }        
        public string AverageCurrentRatePMPM { get; set; }
        public string ProjectedMemberMonths { get; set; }
    }
}
