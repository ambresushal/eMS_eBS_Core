using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ACCM481Arc : Entity
    {
        public string PDPD_ID { get; set; }
        public string ACCM_TYPE { get; set; }
        public Nullable<System.DateTime> ACCM_EFF_DT { get; set; }
        public Nullable<int> ACCM_SEQ_NO { get; set; }
        public Nullable<System.DateTime> ACCM_TERM_DT { get; set; }
        public Nullable<int> ACAC_ACC_NO { get; set; }
        public string ACCM_DESC { get; set; }
        public string ACCM_PFX { get; set; }
        public Nullable<int> ACCM_LOCK_TOKEN { get; set; }
        public Nullable<System.DateTime> ATXR_SOURCE_ID { get; set; }
        public string BatchID { get; set; }
        public Nullable<int> BenefitOfferingID { get; set; }
    }
}
