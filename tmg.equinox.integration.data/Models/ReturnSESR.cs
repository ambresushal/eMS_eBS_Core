using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public class ReturnSESR : Entity
    {
        public string SERL_REL_ID { get; set; }
        public string SESE_ID { get; set; }
        public Int16 SESR_WT_CTR { get; set; }
        public string SESR_OPTS { get; set; }
        public string SERL_DESC { get; set; }
    }
}
