using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnHSQERecords : Entity
    {
        public string HSQE_PFX {get; set; }
        public string HSQE_CD {get; set; }
        public string HSQE_DESC { get; set; }
    }
}
