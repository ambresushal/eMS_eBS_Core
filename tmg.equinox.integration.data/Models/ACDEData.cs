using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class ACDEData : Entity
    {
        public string ACDE_ACC_TYPE { get; set; }
        public Int16 ACAC_ACC_NO { get; set; }
        public string ACDE_DESC { get; set; }
        public string Acronym { get; set; }
        public int ProcessGovernance1up { get; set; }

        public ACDEData Clone()
        {
            return this.MemberwiseClone() as ACDEData;
        }
    }
}
