using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class PDPDData : Entity
    {
        public string PDPD_ID { get; set; }
        public string PDPD_RISK_IND { get; set; }
        public string LOBD_ID { get; set; }
        public string LOBD_ALT_RISK_ID { get; set; }
        public string PDPD_ACC_SFX { get; set; }
        public string PDPD_CAP_POP_LVL { get; set; }
        public Int16 PDPD_CAP_RET_MOS { get; set; }
        public string PDPD_MCTR_CCAT { get; set; }
        public string PDPD_ACC_SHDW_SFX_NVL { get; set; }
        public bool IsSHDW { get; set; }
        public bool IsProductNew { get; set; }
        public bool GenerateNewProduct { get; set; }
        public string StandardProduct { get; set; }
        public string BPL { get; set; }
        public string Product_Category { get; set; }
        public string PERIOD_IND { get; set; }
        public int ProcessGovernance1up { get; set; }
        public string PDPD_OPTS { get; set; }
        public bool IsRetro { get; set; }
    }
}
