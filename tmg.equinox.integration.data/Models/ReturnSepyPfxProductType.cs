using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnSepyPfxProductType:Entity
    {
        public string Old_SEPY_PFX { get; set; }
        public string PROD_TYPE { get; set; }
        public string SEPY_PFX { get; set; }
    }
}
