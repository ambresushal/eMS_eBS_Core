using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class ServicePenaltyData : Entity
    {
        public string PDPD_ID { get; set; }
        public string SESE_ID { get; set; }
        public string BenefitSet { get; set; }
        public string SESP_PEN_IND { get; set; }
        public string SESP_PEN_CALC_IND { get; set; }
        public decimal SESP_PEN_AMT { get; set; }
        public decimal SESP_PEN_PCT { get; set; }
        public decimal SESP_PEN_MAX_AMT { get; set; }
        public string EXCD_ID { get; set; }
        public string BenefitCategory1 { get; set; }
        public string BenefitCategory2 { get; set; }
        public string BenefitCategory3 { get; set; }
        public string POS { get; set; }
        public string SESP_OPTS { get; set; }
        public int ProcessGovernance1up { get; set; }

        public ServicePenaltyData Clone()
        {
            return this.MemberwiseClone() as ServicePenaltyData;
        }
    }
}
