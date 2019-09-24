using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class SERLData : Entity
    {
        public string SERL_REL_ID { get; set; }
        public string SERL_REL_TYPE { get; set; }
        public string SERL_DESC { get; set; }
        public string SERL_REL_PER_IND { get; set; }
        public string SERL_DIAG_IND { get; set; }
        public string SERL_NTWK_IND { get; set; }
        public string SERL_PC_IND { get; set; }
        public string SERL_REF_IND { get; set; }
        public Int16 SERL_PER { get; set; }
        public string SERL_COPAY_IND { get; set; }
        public string SERL_OPTS { get; set; }
        public int ProcessGovernance1up { get; set; }

        public SERLData Clone()
        {
            return this.MemberwiseClone() as SERLData;
        }
    }
}
