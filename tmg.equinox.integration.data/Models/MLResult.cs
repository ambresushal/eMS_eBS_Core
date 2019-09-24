using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public class MLResult : Entity
    {
        public int ProcessGovernance1Up { get; set; }
        public string VersionNumber { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
