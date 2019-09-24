using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class BenefitSet : Entity
    {
        public string PDPD_ID { get; set; }
        public string BenefitSetName { get; set; }
        public string SEPY_PFX { get; set; }
        public string SEPY_PFX_DESC { get; set; }
        public string LTLT_PFX { get; set; }
        public string LTLT_PFX_DESC { get; set; }
        public string DEDE_PFX { get; set; }
        public string DEDE_PFX_DESC { get; set; }
        public int ProcessGovernance1up { get; set; }
        public bool CreateSEPY { get; set; }

        public BenefitSet Clone()
        {
            return this.MemberwiseClone() as BenefitSet;
        }
    }
}
