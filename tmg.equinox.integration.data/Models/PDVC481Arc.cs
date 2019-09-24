using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class PDVC481Arc : Entity
    {
        public int PDVC_Id { get; set; }
        public Nullable<int> ebsInstanceId { get; set; }
        public string PDPD_ID { get; set; }
        public Nullable<short> PDVC_TIER { get; set; }
        public string PDVC_TYPE { get; set; }
        public Nullable<System.DateTime> PDVC_EFF_DT { get; set; }
        public Nullable<short> PDVC_SEQ_NO { get; set; }
        public Nullable<System.DateTime> PDVC_TERM_DT { get; set; }
        public string PDVC_PR_PCP { get; set; }
        public string PDVC_PR_IN { get; set; }
        public string PDVC_PR_PAR { get; set; }
        public string PDVC_PR_NONPAR { get; set; }
        public string PDVC_PC_NR { get; set; }
        public string PDVC_PC_OBT { get; set; }
        public string PDVC_PC_VIOL { get; set; }
        public string PDVC_REF_NR { get; set; }
        public string PDVC_REF_OBT { get; set; }
        public string PDVC_REF_VIOL { get; set; }
        public string PDVC_LOBD_PTR { get; set; }
        public string SEPY_PFX { get; set; }
        public string DEDE_PFX { get; set; }
        public string LTLT_PFX { get; set; }
        public string DPPY_PFX { get; set; }
        public string CGPY_PFX { get; set; }
        public Nullable<short> PDVC_LOCK_TOKEN { get; set; }
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
