using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class DEDEJsonData : Entity
    {
        public string ProductID { get; set; }
        public string BenefitSet { get; set; }
        public int ACAC_ACC_NO { get; set; }
        public string DEDE_DESC { get; set; }
        public int DEDE_RULE { get; set; }
        public int DEDE_REL_ACC_ID { get; set; }
        public string DEDE_COB_OOP_IND { get; set; }
        public string DEDE_SL_IND { get; set; }
        public string DEDE_PERIOD_IND { get; set; }
        public int DEDE_AGG_PERSON { get; set; }
        public int DEDE_AGG_PERSON_CO { get; set; }
        public float DEDE_FAM_AMT { get; set; }
        public float DEDE_FAM_AMT_CO { get; set; }
        public float DEDE_MEME_AMT { get; set; }
        public float DEDE_MEME_AMT_CO { get; set; }
        public string DEDE_OPTS { get; set; }
        public string DEDE_CO_BYPASS { get; set; }
        public string DEDE_MEM_SAL_IND { get; set; }
        public string DEDE_FAM_SAL_IND { get; set; }
        
    }
}
