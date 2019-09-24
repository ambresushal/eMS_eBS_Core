using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class PDPD481Arc : Entity
    {
        public int PDPD_Id { get; set; }
        public Nullable<int> ebsInstanceId { get; set; }
        public string PDPD_ID { get; set; }
        public Nullable<System.DateTime> PDPD_EFF_DT { get; set; }
        public Nullable<System.DateTime> PDPD_TERM_DT { get; set; }
        public string PDPD_RISK_IND { get; set; }
        public string LOBD_ID { get; set; }
        public string LOBD_ALT_RISK_ID { get; set; }
        public string PDPD_ACC_SFX { get; set; }
        public string PDPD_OPTS { get; set; }
        public string PDPD_CAP_POP_LVL { get; set; }
        public Nullable<short> PDPD_CAP_RET_MOS { get; set; }
        public string PDPD_MCTR_CCAT { get; set; }
        public Nullable<short> PDPD_LOCK_TOKEN { get; set; }
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
