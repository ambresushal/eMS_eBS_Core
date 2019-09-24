using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class BSDEData : Entity
    {
        public string BSDE_REC_TYPE { get; set; }
        public string BSDE_TYPE { get; set; }
        public string BSDE_DESC { get; set; }
        public string BSDE_KEYWORD1 { get; set; }
        public string BSDE_KEYWORD2 { get; set; }
        public string BSDE_KEYWORD3 { get; set; }
        public string BSDE_KEYWORD4 { get; set; }
        public string BSDE_KEYWORD5 { get; set; }
        public string BSDE_KEYWORD6 { get; set; }
        public int ProcessGovernance1up { get; set; }

        public BSDEData Clone()
        {
            return this.MemberwiseClone() as BSDEData;
        }
    }
}
