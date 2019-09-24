using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnSERLRecords : Entity
    {
        public int SERL_DISTINCT_ID { get; set; }
        public string SERL_REL_ID { get; set; }
        public string SERL_DESC { get; set; }
        public short SESR_WT_CTR { get; set; }
        public string SERL_REL_TYPE { get; set; }
        public string SERL_REL_PER_IND { get; set; }
        public string SERL_DIAG_IND { get; set; }
        public string SERL_NTWK_IND { get; set; }
        public string SERL_PC_IND { get; set; }
        public string SERL_REF_IND { get; set; }
        public short SERL_PER { get; set; }
        public string SERL_OPTS { get; set; }
        public string SERL_COPAY_IND { get; set; }
        public string SESE_ID { get; set; }
    }
}
