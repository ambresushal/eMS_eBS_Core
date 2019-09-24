using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class LimitServiceData : Entity
    {
        public string PDPD_ID { get; set; }
        public string BenefitSet { get; set; }
        public string SESE_ID { get; set; }
        public Int16? ACAC_ACC_NO { get; set; }
        public string ACDE_DESC { get; set; }
        public int LTSE_WT_CTR { get; set; }
        public string BenefitCategory1 { get; set; }
        public string BenefitCategory2 { get; set; }
        public string BenefitCategory3 { get; set; }
        public string POS { get; set; }
        public int ProcessGovernance1up { get; set; }

        public LimitServiceData Clone()
        {
            return this.MemberwiseClone() as LimitServiceData;
        }
    }
}
