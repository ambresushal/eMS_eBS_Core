using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class SEPYData : Entity
    {
        public int SEPY1Up { get; set; }
        public string SEPY_PFX { get; set; }
        public string PDPD_ID { get; set; }
        public string BenefitSet { get; set; }
        public string SEPYHashCode { get; set; }
        public DateTime? SEPY_EFF_DT { get; set; }
        public DateTime? SEPY_TERM_DT { get; set; }
        public string SESE_ID { get; set; }
        public string SESE_RULE { get; set; }
        public string SEPY_EXP_CAT { get; set; }
        public string SEPY_ACCT_CAT { get; set; }
        public string SEPY_OPTS { get; set; }
        public string SESE_RULE_ALT { get; set; }
        public string SESE_RULE_ALT_COND { get; set; }
        public int ProcessGovernance1Up { get; set; }
    }
}
