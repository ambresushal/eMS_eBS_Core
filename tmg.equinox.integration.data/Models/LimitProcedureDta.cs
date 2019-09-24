using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class LimitProcedureData : Entity
    {
        public string PDPD_ID { get; set; }
        public string BenefitSet { get; set; }
        public string ACDE_DESC { get; set; }
        public string LTIP_IPCD_ID_HIGH { get; set; }
        public string LTIP_IPCD_ID_LOW { get; set; }
        public int ProcessGovernance1up { get; set; }
        public Int16 ACAC_ACC_NO { get; set; }

        public LimitProcedureData Clone()
        {
            return this.MemberwiseClone() as LimitProcedureData;
        }
    }
}
