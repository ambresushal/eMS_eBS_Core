using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ACDE481Arc : Entity
    {
        public int ACDE_Id { get; set; }
        public Nullable<int> ebsInstanceId { get; set; }
        public string ACDE_ACC_TYPE { get; set; }
        public Nullable<short> ACAC_ACC_NO { get; set; }
        public string ACDE_DESC { get; set; }
        public Nullable<short> ACDE_LOCK_TOKEN { get; set; }
        public Nullable<System.DateTime> ATXR_SOURCE_ID { get; set; }
        public string BatchID { get; set; }
        public string NewBatchID { get; set; }
        public Nullable<System.DateTime> ArchiveDate { get; set; }
        public Nullable<int> BenefitOfferingID { get; set; }
    }
}
