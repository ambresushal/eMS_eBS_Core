using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class HSQE481Stg : Entity
    {
        public string HSQE_PFX { get; set; }
        public string HSQE_CD { get; set; }
        public string HSQE_DESC { get; set; }
        public int HSQE_LOCK_TOKEN { get; set; }
        public Nullable<System.DateTime> ATXR_SOURCE_ID { get; set; }
    }
}
