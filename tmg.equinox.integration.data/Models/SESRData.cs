using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class SESRData : Entity
    {
        public string SERL_REL_ID { get; set; }
        public string SESE_ID { get; set; }
        public Int16 SESR_WT_CTR { get; set; }
        public string SESR_OPTS { get; set; }
        public int ProcessGovernance1up { get; set; }

        public SESRData Clone()
        {
            return this.MemberwiseClone() as SESRData;
        }
    }
}
