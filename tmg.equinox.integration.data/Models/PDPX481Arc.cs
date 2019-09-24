using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class PDPX481Arc : Entity
    {
        public int PDPX_Id { get; set; }
        public Nullable<int> ebsInstanceId { get; set; }
        public string PDBC_PFX { get; set; }
        public string PDBC_TYPE { get; set; }
        public string PDPX_DESC { get; set; }
        public Nullable<int> PDPX_LOCK_TOKEN { get; set; }
        public Nullable<System.DateTime> ATXR_SOURCE_ID { get; set; }
        public string BatchID { get; set; }
        public string NewBatchID { get; set; }
        public Nullable<System.DateTime> ArchiveDate { get; set; }
        public Nullable<int> BenefitOfferingID { get; set; }
    }
}
