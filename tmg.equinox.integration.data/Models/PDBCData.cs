using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class PDBCData : Entity
    {
        public string PDPD_ID { get; set; }
        public string PDBC_TYPE { get; set; }
        public string PDBC_PFX { get; set; }
        public int ProcessGovernance1up { get; set; }
        public string PDBC_OPTS { get; set; }
        public DateTime? PDBC_EFF_DT { get; set; }
        public DateTime? PDBC_TERM_DT { get; set; }
        public bool CreateNewPFX { get; set; }
        public bool IsPFXNew { get; set; }

        public PDBCData Clone()
        {
            return this.MemberwiseClone() as PDBCData;
        }
    }
}
