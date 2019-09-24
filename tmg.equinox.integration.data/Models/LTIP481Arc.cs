using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class LTIP481Arc : Entity
    {
        public int LTPI_Id { get; set; }
        public Nullable<int> ebsInstanceId { get; set; }
        public string LTLT_PFX { get; set; }
        public Nullable<short> ACAC_ACC_NO { get; set; }
        public string LTIP_IPCD_ID_LOW { get; set; }
        public string LTIP_IPCD_ID_HIGH { get; set; }
        public Nullable<short> LTIP_LOCK_TOKEN { get; set; }
        public Nullable<System.DateTime> ATXR_SOURCE_ID { get; set; }
        public string BatchID { get; set; }
        public string NewBatchID { get; set; }
        public Nullable<System.DateTime> ArchiveDate { get; set; }
        public Nullable<int> BenefitOfferingID { get; set; }
    }
}
