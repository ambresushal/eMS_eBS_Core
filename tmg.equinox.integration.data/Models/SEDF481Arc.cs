using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class SEDF481Arc : Entity
    {
        public int SEDF_Id { get; set; }
        public Nullable<int> ebsInstanceId { get; set; }
        public string PDBC_PFX { get; set; }
        public Nullable<System.DateTime> SEDF_EFF_DT { get; set; }
        public string SESE_ID { get; set; }
        public Nullable<System.DateTime> SEDF_TERM_DT { get; set; }
        public string SEPC_PRICE_ID { get; set; }
        public string SEDF_TYPE { get; set; }
        public string SEDF_UM_REQ_IND { get; set; }
        public string SEDF_UM_BP_REF_IND { get; set; }
        public string SEDF_UM_MAT_IND { get; set; }
        public string SEDF_RWH_IND { get; set; }
        public string SEDF_CAP_IND { get; set; }
        public string SEDF_OPTS { get; set; }
        public Nullable<short> SEDF_UM_AUTH_WAIVE { get; set; }
        public Nullable<decimal> SEDF_UM_AUTH_AMT { get; set; }
        public Nullable<short> SEDF_UM_UNITS_WAIVE { get; set; }
        public Nullable<short> SEDF_AUTH_WV_DAYS { get; set; }
        public Nullable<short> SEDF_LOCK_TOKEN { get; set; }
        public Nullable<System.DateTime> ATXR_SOURCE_ID { get; set; }
        public string BatchID { get; set; }
        public string NewBatchID { get; set; }
        public Nullable<System.DateTime> ArchiveDate { get; set; }
        public Nullable<int> BenefitOfferingID { get; set; }
    }
}
