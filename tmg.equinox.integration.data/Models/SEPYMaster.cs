using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.data.Models
{
    public partial class SEPYMaster:Entity
    {
        public int ID { get; set; }
        public string SEPY_PFX { get; set; }
        public System.DateTime? SEPY_EFF_DT { get; set; }
        public string SESE_ID { get; set; }
        public System.DateTime? SEPY_TERM_DT { get; set; }
        public string SESE_RULE { get; set; }
        public string SEPY_EXP_CAT { get; set; }
        public string SEPY_ACCT_CAT { get; set; }
        public string SEPY_OPTS { get; set; }
        public string SESE_RULE_ALT { get; set; }
        public string SESE_RULE_ALT_COND { get; set; }
        public Int16? SEPY_LOCK_TOKEN { get; set; }
        public System.DateTime? ATXR_SOURCE_ID { get; set; }
        public string Hashcode { get; set; }
        
    }
}
