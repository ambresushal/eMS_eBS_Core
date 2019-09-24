using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnSESPRecords : Entity
    {
        public int SESP_DISTINCT_ID { get; set; }
        public string MESSAGEVALUE { get; set; }
        public string SESE_RULE { get; set; }
        public string SESP_PEN_IND { get; set; }
        public string SESP_PEN_CALC_IND { get; set; }
        public decimal SESP_PEN_AMT { get; set; }
        public decimal SESP_PEN_PCT { get; set; }
        public decimal SESP_PEN_MAX_AMT { get; set; }
        public string EXCD_ID { get; set; }
        public string SERL_REL_ID { get; set; }
        public string SESP_OPTS { get; set; }
    }
}
