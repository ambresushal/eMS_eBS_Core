using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class DEDEData : Entity
    {
        public string PDPD_ID { get; set; }
        public string BenefitSet { get; set; }
        public Int16 ACAC_ACC_NO { get; set; }
        public string DEDE_DESC { get; set; }
        public string DEDE_RULE { get; set; }
        public Int16 DEDE_REL_ACC_ID { get; set; }
        public string DEDE_COB_OOP_IND { get; set; }
        public string DEDE_SL_IND { get; set; }
        public string DEDE_PERIOD_IND { get; set; }
        public Int16 DEDE_AGG_PERSON { get; set; }
        public decimal DEDE_FAM_AMT { get; set; }
        public decimal DEDE_MEME_AMT { get; set; }
        public Int16 DEDE_AGG_PERSON_CO { get; set; }
        public decimal DEDE_FAM_AMT_CO { get; set; }
        public decimal DEDE_MEME_AMT_CO { get; set; }
        public string DEDE_CO_BYPASS { get; set; }
        public string DEDE_MEM_SAL_IND { get; set; }
        public string DEDE_FAM_SAL_IND { get; set; }
        public string EBCLSelection { get; set; }
        public int ProcessGovernance1up { get; set; }
        public string DEDE_OPTS { get; set; }
        public DEDEData Clone()
        {
            return this.MemberwiseClone() as DEDEData;
        }
    }
}
