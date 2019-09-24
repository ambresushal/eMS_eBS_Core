using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.data.Models
{
    public partial class BSDLSRC: Entity
    {
        public string PDBC_PFX { get; set; }
        public string BSDL_TYPE { get; set; }
        public string BSDL_NTWK_IND { get; set; }
        public DateTime? BSDL_EFF_DT { get; set; }
        public DateTime? BSDL_TERM_DT { get; set; }
        public int? ProcessGovernance1up { get; set; }
        public decimal? BSDL_COPAY_AMT { get; set; }
        public decimal? BSDL_DEDE_AMT { get; set; }
        public decimal? BSDL_COIN_PCT { get; set; }
        public decimal? BSDL_LTLT_AMT { get; set; }
        public string BSDL_LT_TYPE { get; set; }
        public string BSDL_LT_PERIOD { get; set; }
        public string BSDL_LT_COUNTER { get; set; }
        public Int16? BSDL_TIER { get; set; }
        public string BSDL_COV_IND { get; set; }
        public decimal? BSDL_STOPLOSS_AMT { get; set; }
        public string BSDL_STOPLOSS_TYPE { get; set; }
        public Int16? BSDL_BEG_MMDD { get; set; }
        public string BSDL_USER_LABEL1 { get; set; }
        public string BSDL_USER_DATA1 { get; set; }
        public string BSDL_USER_LABEL2 { get; set; }
        public string BSDL_USER_DATA2 { get; set; }
        public string BSDL_USER_LABEL3 { get; set; }
        public string BSDL_USER_DATA3 { get; set; }
        public string BSDL_USER_LABEL4 { get; set; }
        public string BSDL_USER_DATA4 { get; set; }
        public string BSDL_USER_LABEL5 { get; set; }
        public string BSDL_USER_DATA5 { get; set; }
        public string BSDL_USER_LABEL6 { get; set; }
        public string BSDL_USER_DATA6 { get; set; } 
    }
}
