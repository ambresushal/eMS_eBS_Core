using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class BenefitSummaryInfoData : Entity
    {
        public string PDPD_ID { get; set; }
        public string BSBS_TYPE { get; set; }
        public string BSBS_DESC { get; set; }
        public byte[] BSTX_TEXT { get; set; }
        public string BSTX_TEXT_varchar { get; set; }
        public Int16 BSTX_SEQ_NO { get; set; }
        public int ProcessGovernance1up { get; set; }

        public BenefitSummaryInfoData Clone()
        {
            return this.MemberwiseClone() as BenefitSummaryInfoData;
        }
    }
}
