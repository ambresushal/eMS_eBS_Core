using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class DEDETRANS : Entity
    {
        //public int DEDEId { get; set; }
        public string DEDE_PFX { get; set; }
        public int ACAC_ACC_NO { get; set; }
        public string DEDE_DESC { get; set; }
        public int DEDE_RULE { get; set; }
        public int DEDE_REL_ACC_ID { get; set; }
        public string DEDE_COB_OOP_IND { get; set; }
        public string DEDE_SL_IND { get; set; }
        public string DEDE_PERIOD_IND { get; set; }
        public int DEDE_AGG_PERSON { get; set; }
        public int DEDE_AGG_PERSON_CO { get; set; }
        public decimal DEDE_FAM_AMT { get; set; }
        public decimal DEDE_FAM_AMT_CO { get; set; }
        public decimal DEDE_MEME_AMT { get; set; }
        public decimal DEDE_MEME_AMT_CO { get; set; }
        public string DEDE_OPTS { get; set; }
        public string DEDE_CO_BYPASS { get; set; }
        public string DEDE_MEM_SAL_IND { get; set; }
        public string DEDE_FAM_SAL_IND { get; set; }
        public int DEDE_LOCK_TOKEN { get; set; }
        public System.DateTime ATXR_SOURCE_ID { get; set; }
        public Nullable<System.DateTime> SYS_LAST_UPD_DTM { get; set; }
        public string SYS_USUS_ID { get; set; }
        public string SYS_DBUSER_ID { get; set; }
        public string BatchID { get; set; }
        public int Action { get; set; }
        //public string Hashcode { get; set; }
    }
}
