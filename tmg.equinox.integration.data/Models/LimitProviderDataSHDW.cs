using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class LimitProviderDataSHDW : Entity
    {
        public string PDPD_ID { get; set; }
        public string BenefitSet { get; set; }
        public string ACDE_DESC { get; set; }
        public string PRPR_MCTR_TYPE { get; set; }
        public int ProcessGovernance1up { get; set; }
        public Int16 ACAC_ACC_NO { get; set; }

        public LimitProviderDataSHDW Clone()
        {
            return this.MemberwiseClone() as LimitProviderDataSHDW;
        }
    }
}
