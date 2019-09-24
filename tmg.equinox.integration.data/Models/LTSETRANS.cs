using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class LTSETRANS : Entity
    {
        public int LTSEId { get; set; }
        public string LTLT_PFX { get; set; }
        public int ACAC_ACC_NO { get; set; }
        public string SESE_ID { get; set; }
        public int LTSE_WT_CTR { get; set; }
        public int LTSE_LOCK_TOKEN { get; set; }
        public System.DateTime ATXR_SOURCE_ID { get; set; }
        public Nullable<System.DateTime> SYS_LAST_UPD_DTM { get; set; }
        public string SYS_USUS_ID { get; set; }
        public string SYS_DBUSER_ID { get; set; }
        public string BatchID { get; set; }
        public Nullable<int> BenefitOfferingID { get; set; }
    }
}
