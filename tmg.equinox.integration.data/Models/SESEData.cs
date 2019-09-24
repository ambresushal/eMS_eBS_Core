using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class SESEData :Entity
    {
        public string SESE_ID { get; set; }
        public int ProcessGovernance1up { get; set; }

        public SESEData Clone()
        {
            return this.MemberwiseClone() as SESEData;
        }
    }
}
