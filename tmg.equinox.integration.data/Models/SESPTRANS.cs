using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class SESPTRANS : Entity
    {
        public string SESE_ID { get; set; }
        public string SESE_RULE { get; set; }
        public string SESP_PEN_IND { get; set; }
        public string SESP_PEN_CALC_IND { get; set; }
        public decimal SESP_PEN_AMT { get; set; }
        public decimal SESP_PEN_PCT { get; set; }
        public decimal SESP_PEN_MAX_AMT { get; set; }
        public string EXCD_ID { get; set; }
        public string SERL_REL_ID { get; set; }
        public string SESP_OPTS { get; set; }
        public short SESP_LOCK_TOKEN { get; set; }
        public System.DateTime ATXR_SOURCE_ID { get; set; }
        public string BatchID { get; set; }
        public int Action { get; set; }
        public string Hashcode { get; set; }
    }
}
