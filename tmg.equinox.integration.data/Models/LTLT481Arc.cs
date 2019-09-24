using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class LTLT481Arc : Entity
    {
        public int LTLT_Id { get; set; }
        public Nullable<int> ebsInstanceId { get; set; }
        public string LTLT_PFX { get; set; }
        public Nullable<short> ACAC_ACC_NO { get; set; }
        public string LTLT_DESC { get; set; }
        public string LTLT_CAT { get; set; }
        public string LTLT_LEVEL { get; set; }
        public string LTLT_PERIOD_IND { get; set; }
        public string LTLT_RULE { get; set; }
        public string LTLT_IX_IND { get; set; }
        public string LTLT_IX_TYPE { get; set; }
        public string EXCD_ID { get; set; }
        public Nullable<decimal> LTLT_AMT1 { get; set; }
        public Nullable<decimal> LTLT_AMT2 { get; set; }
        public string LTLT_OPTS { get; set; }
        public string LTLT_SAL_IND { get; set; }
        public Nullable<short> LTLT_DAYS { get; set; }
        public Nullable<short> WMDS_SEQ_NO { get; set; }
        public Nullable<short> LTLT_LOCK_TOKEN { get; set; }
        public Nullable<System.DateTime> ATXR_SOURCE_ID { get; set; }
        public Nullable<System.DateTime> SYS_LAST_UPD_DTM { get; set; }
        public string SYS_USUS_ID { get; set; }
        public string SYS_DBUSER_ID { get; set; }
        public string BatchID { get; set; }
        public string NewBatchID { get; set; }
        public Nullable<System.DateTime> ArchiveDate { get; set; }
        public Nullable<int> BenefitOfferingID { get; set; }
    }
}
