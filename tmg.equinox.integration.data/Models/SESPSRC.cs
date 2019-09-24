using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.data.Models
{
    public partial class SESPSRC:Entity
    {        
        public string SESE_RULE { get; set; }
        public string SESP_PEN_IND { get; set; }
        public string SESP_PEN_CALC_IND { get; set; }
        public decimal? SESP_PEN_AMT { get; set; }
        public decimal? SESP_PEN_PCT { get; set; }
        public decimal? SESP_PEN_MAX_AMT { get; set; }
        public string EXCD_ID { get; set; }
        public string SERL_REL_ID { get; set; }
        public string SESP_OPTS { get; set; }
        public Int16? SESP_LOCK_TOKEN { get; set; }
        public System.DateTime? ATXR_SOURCE_ID { get; set; }       
        public int? ProcessGovernance1up { get; set; }        
    }
}
