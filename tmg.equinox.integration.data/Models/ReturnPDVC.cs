using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnPDVC : Entity
    {
        public string PDPD_ID { get; set; }
        public string SEPY_PFX { get; set; }
        //PDVC Distinct table
        public int PDVC_ID { get; set; }
        public string NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string PDVC_TYPE { get; set; }
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
    }
}
