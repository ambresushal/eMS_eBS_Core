using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class SESRTRANS : Entity
    {
        public string SERL_REL_ID { get; set; }
        public string SESE_ID { get; set; }
        public short SESR_WT_CTR { get; set; }
        public string SESR_OPTS { get; set; }
        public short SESR_LOCK_TOKEN { get; set; }
        public System.DateTime ATXR_SOURCE_ID { get; set; }
        public string BatchID { get; set; }
        public int Action { get; set; }
       // public string Hashcode { get; set; }
    }
}
