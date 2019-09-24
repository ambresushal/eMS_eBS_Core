using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class SEDSSRC : Entity
    {
        public string sese_id { get; set; }
        public string seds_desc { get; set; }
        public string seds_type { get; set; }
        public string sese_id_xlow { get; set; }
        public string seds_desc_xlow { get; set; }
        public Int16? seds_lock_token { get; set; }
        public Nullable<System.DateTime> atxr_source_id { get; set; }
        public Nullable<System.DateTime> rec_creat_dt { get; set; }
        public Nullable<System.DateTime> rec_updt_dt { get; set; }
        public string SEDS_MCTR_RPT_TYPE_NVL { get; set; }
        public string BatchID { get; set; } 
        public int? ProcessGovernance1up { get; set; }
        public int? Action { get; set; }
        public string Hashcode { get; set; }
    }


}
