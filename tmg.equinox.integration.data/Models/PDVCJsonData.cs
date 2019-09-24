using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class PDVCJsonData : Entity
    {
        public string ProductID { get; set; }
        public string BenefitSet { get; set; }
        public int PDVC_TIER { get; set; }
        public string PDVC_TYPE { get; set; }
        public DateTime PDVC_EFF_DT  { get; set; }
        public int PDVC_SEQ_NO { get; set; }
        public DateTime PDVC_TERM_DT  { get; set; }
        public string PDVC_PR_PCP { get; set; }
        public string PDVC_PR_IN { get; set; }
        public string PDVC_PR_PAR  { get; set; }
        public string PDVC_PR_NONPAR  { get; set; }
        public string PDVC_PC_NR  { get; set; }
        public string PDVC_PC_OBT { get; set; }
        public string PDVC_PC_VIOL { get; set; }
        public string PDVC_REF_NR { get; set; }
        public string PDVC_REF_OBT { get; set; }
        public string PDVC_REF_VIOL { get; set; }
        public string PDVC_LOBD_PTR { get; set; }
    }
}
