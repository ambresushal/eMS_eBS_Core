using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.data.Models
{
    public partial class LTLTREP : Entity
    {
        public string LTLT_PFX { get; set; }
        public int ACAC_ACC_NO { get; set; }
        public string LTLT_DESC { get; set; }
        public string LTLT_CAT { get; set; }
        public string LTLT_LEVEL { get; set; }
        public string LTLT_PERIOD_IND { get; set; }
        public string LTLT_RULE { get; set; }
        public string LTLT_IX_IND { get; set; }
        public string LTLT_IX_TYPE { get; set; }
        public string EXCD_ID { get; set; }
        public double LTLT_AMT1 { get; set; }
        public double LTLT_AMT2 { get; set; }
        public string LTLT_OPTS { get; set; }
        public string LTLT_SAL_IND { get; set; }
        public int LTLT_DAYS { get; set; }
        public int WMDS_SEQ_NO { get; set; }
        public int LTLT_LOCK_TOKEN { get; set; }
        public System.DateTime ATXR_SOURCE_ID { get; set; }
        public Nullable<System.DateTime> SYS_LAST_UPD_DTM { get; set; }
        public string SYS_USUS_ID { get; set; }
        public string SYS_DBUSER_ID { get; set; }
        public string LTLT_EXCL_DED_IND_NVL { get; set; }
        public string Hashcode { get; set; }
    }
}
