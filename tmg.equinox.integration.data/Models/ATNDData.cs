using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class ATNDData : Entity
    {
        public string PDPD_ID { get; set; }
        public Int16 ATND_SEQ_NO { get; set; }
        public byte[] ATND_TEXT { get; set; }
        public int ProcessGovernance1Up { get; set; }

        public ATNDData Clone()
        {
            return this.MemberwiseClone() as ATNDData;
        }
    }
}
