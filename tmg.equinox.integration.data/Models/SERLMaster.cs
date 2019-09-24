using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class SERLMaster : Entity
    {
        //public int SERLId { get; set; }
        public string SERL_REL_ID { get; set; }
        public string SERL_REL_TYPE { get; set; }
        public string SERL_REL_PER_IND { get; set; }
        public string SERL_DIAG_IND { get; set; }
        public string SERL_NTWK_IND { get; set; }
        public string SERL_PC_IND { get; set; }
        public string SERL_REF_IND { get; set; }
        public Int16? SERL_PER { get; set; }
        public string SERL_OPTS { get; set; }
        public string SERL_COPAY_IND { get; set; }
        public string SERL_DESC { get; set; }
        //public Int16? SERL_LOCK_TOKEN { get; set; }
        //public System.DateTime? ATXR_SOURCE_ID { get; set; }
        //public Nullable<System.DateTime> SYS_LAST_UPD_DTM { get; set; }
        //public string SYS_USUS_ID { get; set; }
        //public string SYS_DBUSER_ID { get; set; }
        //public string BatchID { get; set; }
        //public int Action { get; set; }
        //public string Hashcode { get; set; }
    }
}
