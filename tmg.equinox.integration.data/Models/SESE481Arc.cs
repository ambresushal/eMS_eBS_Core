using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class SESE481Arc : Entity
    {
        public int SESE_Id { get; set; }
        public Nullable<int> ebsInstanceId { get; set; }
        public string SESE_ID { get; set; }
        public string SESE_RULE { get; set; }
        public string SESE_DESC { get; set; }
        public string SESE_CM_IND { get; set; }
        public string SESE_PA_AMT_REQ { get; set; }
        public string SESE_PA_UNIT_REQ { get; set; }
        public string SESE_PA_PROC_REQ { get; set; }
        public string SESE_VALID_SEX { get; set; }
        public string SESE_SEX_EXCD_ID { get; set; }
        public Nullable<short> SESE_MIN_AGE { get; set; }
        public Nullable<short> SESE_MAX_AGE { get; set; }
        public string SESE_AGE_EXCD_ID { get; set; }
        public string SESE_COV_TYPE { get; set; }
        public string SESE_COV_EXCD_ID { get; set; }
        public string SESE_RULE_TYPE { get; set; }
        public string SESE_CALC_IND { get; set; }
        public string SERL_REL_ID { get; set; }
        public string SESE_OPTS { get; set; }
        public Nullable<short> WMDS_SEQ_NO { get; set; }
        public string SESE_ID_XLOW { get; set; }
        public string SESE_DESC_XLOW { get; set; }
        public string SESE_DIS_EXCD_ID { get; set; }
        public Nullable<decimal> SESE_MAX_CPAY_PCT { get; set; }
        public string SESE_FSA_REIMB_IND { get; set; }
        public string SESE_HSA_REIMB_IND { get; set; }
        public string SESE_HRA_DED_IND { get; set; }
        public Nullable<short> SESE_LOCK_TOKEN { get; set; }
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
