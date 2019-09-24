using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class PDPDBatch : Entity
    {
        public int PDPDBatch_ID { get; set; }
        public string BatchID { get; set; }
        public string PDPD_ID { get; set; }
        public Nullable<System.DateTime> PDPD_EFF_DT { get; set; }
        public string Flag { get; set; }
        public string FolderVersion { get; set; }
    }
}
