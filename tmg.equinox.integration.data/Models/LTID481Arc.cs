using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class LTID481Arc : Entity
    {
        public int LTID_Id { get; set; }
        public Nullable<int> ebsInstanceId { get; set; }
        public string LTLT_PFX { get; set; }
        public Nullable<int> ACAC_ACC_NO { get; set; }
        public string IDCD_ID_REL { get; set; }
        public Nullable<int> LTID_LOCK_TOKEN { get; set; }
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