using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public partial class MigrationBenefitSets : Entity
    {
        public string PDPD_ID { set; get; }
        public Nullable<System.DateTime> EFF_DT { set; get; }
        public string SEPY_PFX { set; get; }
        public string LTLT_PFX { set; get; }
        public string DEDE_PFX { set; get; }
        public string BenefitSetName { set; get; }
        public string LOBD_ID { set; get; }
        public int PDVC_TIER { set; get; }
        public int PDVC_SEQ_NO { set; get; }
        public string SEPY_SHDW_PFX_NVL { set; get; }
        public string LTLT_SHDW_PFX_NVL { set; get; }
        public string DEDE_SHDW_PFX_NVL { set; get; }
        public int GroupID { set; get; }
    }
}
