using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class EBCLData : Entity
    {
        public string PDPD_ID { get; set; }
        public string BenefitSet { get; set; }
        public string EBCL_TYPE { get; set; }
        public string EBCL_YEAR_IND { get; set; }
        public Int16 ACAC_ACC_NO { get; set; }
        public string EBCL_ZERO_AMT_IND { get; set; }
        public int ProcessGovernance1up { get; set; }

        public EBCLData Clone()
        {
            return this.MemberwiseClone() as EBCLData;
        }
    }
}
