using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.data.Models
{
    public partial class SESRSRC:Entity
    {
        public string SERL_REL_ID { get; set; }
        public string SESE_ID { get; set; }
        public Int16? SESR_WT_CTR { get; set; }
        public string SESR_OPTS { get; set; }
        public Int16? SESR_LOCK_TOKEN { get; set; }
        public System.DateTime? ATXR_SOURCE_ID { get; set; }        
        public int? ProcessGovernance1up { get; set; }
        public int Action { get; set; }
    }
}
