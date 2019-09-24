using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class SEPY481Arc : Entity
    {
        public int SEPY_Id { get; set; }
        public Nullable<int> ebsInstanceId { get; set; }
        public string SEPY_PFX { get; set; }
        public Nullable<System.DateTime> SEPY_EFF_DT { get; set; }
        public string SESE_ID { get; set; }
        public Nullable<System.DateTime> SEPY_TERM_DT { get; set; }
        public string SESE_RULE { get; set; }
        public string SEPY_EXP_CAT { get; set; }
        public string SEPY_ACCT_CAT { get; set; }
        public string SEPY_OPTS { get; set; }
        public string SESE_RULE_ALT { get; set; }
        public string SESE_RULE_ALT_COND { get; set; }
        public Nullable<short> SEPY_LOCK_TOKEN { get; set; }
        public Nullable<System.DateTime> ATXR_SOURCE_ID { get; set; }
        public string BatchID { get; set; }
        public string NewBatchID { get; set; }
        public Nullable<System.DateTime> ArchiveDate { get; set; }
        public Nullable<int> BenefitOfferingID { get; set; }
    }
}
