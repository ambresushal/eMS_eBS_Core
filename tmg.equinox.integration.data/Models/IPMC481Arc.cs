using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class IPMC481Arc : Entity
    {
        public int IPMC_Id { get; set; }
        public Nullable<int> ebsInstanceId { get; set; }
        public string PDBC_PFX { get; set; }
        public string IPCD_LOW { get; set; }
        public Nullable<System.DateTime> IPMC_EFF_DT { get; set; }
        public Nullable<System.DateTime> IPMC_TERM_DT { get; set; }
        public string IPCD_HIGH { get; set; }
        public string IPMC_UM_IND { get; set; }
        public Nullable<decimal> IPMC_UM_AUTH_AMT { get; set; }
        public Nullable<short> IPMC_UM_UNITS_WAIVE { get; set; }
        public Nullable<short> IPMC_AUTH_WV_DAYS { get; set; }
        public Nullable<short> IPMC_LOCK_TOKEN { get; set; }
        public Nullable<System.DateTime> ATXR_SOURCE_ID { get; set; }
        public string BatchID { get; set; }
        public string NewBatchID { get; set; }
        public Nullable<System.DateTime> ArchiveDate { get; set; }
        public Nullable<int> BenefitOfferingID { get; set; }
    }
}
