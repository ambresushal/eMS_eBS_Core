using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class SESR481Arc : Entity
    {
        public int SESR_Id { get; set; }
        public Nullable<int> ebsInstanceId { get; set; }
        public string SERL_REL_ID { get; set; }
        public string SESE_ID { get; set; }
        public Nullable<short> SESR_WT_CTR { get; set; }
        public string SESR_OPTS { get; set; }
        public Nullable<short> SESR_LOCK_TOKEN { get; set; }
        public Nullable<System.DateTime> ATXR_SOURCE_ID { get; set; }
        public string BatchID { get; set; }
        public string NewBatchID { get; set; }
        public Nullable<System.DateTime> ArchiveDate { get; set; }
        public Nullable<int> BenefitOfferingID { get; set; }
    }
}
