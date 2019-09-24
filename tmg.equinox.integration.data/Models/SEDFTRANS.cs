using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class SEDFTRANS : Entity
    {
        public string PDBC_PFX { get; set; }
        public System.DateTime SEDF_EFF_DT { get; set; }
        public string SESE_ID { get; set; }
        public System.DateTime SEDF_TERM_DT { get; set; }
        public string SEPC_PRICE_ID { get; set; }
        public string SEDF_TYPE { get; set; }
        public string SEDF_UM_REQ_IND { get; set; }
        public string SEDF_UM_BP_REF_IND { get; set; }
        public string SEDF_UM_MAT_IND { get; set; }
        public string SEDF_RWH_IND { get; set; }
        public string SEDF_CAP_IND { get; set; }
        public string SEDF_OPTS { get; set; }
        public short SEDF_UM_AUTH_WAIVE { get; set; }
        public decimal SEDF_UM_AUTH_AMT { get; set; }
        public short SEDF_UM_UNITS_WAIVE { get; set; }
        public short SEDF_AUTH_WV_DAYS { get; set; }
        public short SEDF_LOCK_TOKEN { get; set; }
        public System.DateTime ATXR_SOURCE_ID { get; set; }
        public string BatchID { get; set; }
        public Nullable<int> BenefitOfferingID { get; set; }
        public int Action { get; set; }
        public string Hashcode { get; set; }
    }
}
