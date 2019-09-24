using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class LimitDiagnosisDataSHDW : Entity
    {
        public string PDPD_ID { get; set; }
        public string BenefitSet { get; set; }
        public string ACDE_DESC { get; set; }
        public string IDCD_ID_REL { get; set; }
        public string IDCD_TYPE { get; set; }
        public int ProcessGovernance1up { get; set; }
        public Int16 ACAC_ACC_NO { get; set; }

        public LimitDiagnosisDataSHDW Clone()
        {
            return this.MemberwiseClone() as LimitDiagnosisDataSHDW;
        }
    }
}
