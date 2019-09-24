using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class SEPYTRANS : Entity
    {
        public string SEPY_PFX { get; set; }
        public System.DateTime SEPY_EFF_DT { get; set; }
        public string SESE_ID { get; set; }
        public System.DateTime SEPY_TERM_DT { get; set; }
        public string SESE_RULE { get; set; }
        public string SEPY_EXP_CAT { get; set; }
        public string SEPY_ACCT_CAT { get; set; }
        public string SEPY_OPTS { get; set; }
        public string SESE_RULE_ALT { get; set; }
        public string SESE_RULE_ALT_COND { get; set; }
        public short SEPY_LOCK_TOKEN { get; set; }
        public System.DateTime ATXR_SOURCE_ID { get; set; }
        public int ID { get; set; }
        public string BatchID { get; set; }
        public int Action { get; set; }
        public string Hashcode { get; set; }
    }
}
