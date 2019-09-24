using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class PDVCData : Entity
    {
        public string PDPD_ID { get; set; }
        public string BenefitSet { get; set; }
        public Int16 PDVC_TIER { get; set; }
        public string PDVC_TYPE { get; set; }
        public Int16 PDVC_SEQ_NO { get; set; }
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
        public int ProcessGovernance1up { get; set; }
        public DateTime? PDVC_EFF_DT { get; set; }
        public DateTime? PDVC_TERM_DT { get; set; }

        public string SEPY_PFX { get; set; }
        public string DEDE_PFX { get; set; }
        public string LTLT_PFX { get; set; }
        public string DPPY_PFX { get; set; }
        public string CGPY_PFX { get; set; }
        public string BSME_PFX { get; set; }
        public string SEPY_SHDW_PFX_NVL { get; set; }
        public string DEDE_SHDW_PFX_NVL { get; set; }
        public string LTLT_SHDW_PFX_NVL { get; set; }
        public string DPPY_SHDW_PFX_NVL { get; set; }
        public string CGPY_SHDW_PFX_NVL { get; set; }
        public string DEDEHashcode { get; set; }
        public string LTLTHashcode { get; set; }
        public string LTSEHashcode { get; set; }
        public string LTIPHashcode { get; set; }
        public string LTIDHashcode { get; set; }
        public string LTPRHashcode { get; set; }
        public string PDVCHashcode { get; set; }
        public string SEPYHashCode { get; set; }
        public string LTLTMainHash { get; set; }
        public string StandardProduct { get; set; }
        public bool ImSHDW { get; set; }
        public bool ImNotInQueue { get; set; }

        public PDVCData Clone()
        {
            return this.MemberwiseClone() as PDVCData;
        }
    }
}
