using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class BSDE481Stg : Entity
    {
        public string BSDE_REC_TYPE { get; set; }
        public string BSDE_TYPE { get; set; }
        public string BSDE_DESC { get; set; }
        public string BSDE_KEYWORD1 { get; set; }
        public string BSDE_KEYWORD2 { get; set; }
        public string BSDE_KEYWORD3 { get; set; }
        public string BSDE_KEYWORD4 { get; set; }
        public string BSDE_KEYWORD5 { get; set; }
        public string BSDE_KEYWORD6 { get; set; }
        public int BSDE_LOCK_TOKEN { get; set; }
        public Nullable<System.DateTime> ATXR_SOURCE_ID { get; set; }
    }
}
