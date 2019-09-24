using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class SESP481Arc : Entity
    {
        public int SESP_Id { get; set; }
        public Nullable<int> ebsInstanceId { get; set; }
        public string SESE_ID { get; set; }
        public string SESE_RULE { get; set; }
        public string SESP_PEN_IND { get; set; }
        public string SESP_PEN_CALC_IND { get; set; }
        public Nullable<decimal> SESP_PEN_AMT { get; set; }
        public Nullable<decimal> SESP_PEN_PCT { get; set; }
        public Nullable<decimal> SESP_PEN_MAX_AMT { get; set; }
        public string EXCD_ID { get; set; }
        public string SERL_REL_ID { get; set; }
        public string SESP_OPTS { get; set; }
        public Nullable<short> SESP_LOCK_TOKEN { get; set; }
        public Nullable<System.DateTime> ATXR_SOURCE_ID { get; set; }
        public string BatchID { get; set; }
        public string NewBatchID { get; set; }
        public Nullable<System.DateTime> ArchiveDate { get; set; }
        public Nullable<int> BenefitOfferingID { get; set; }
    }
}
