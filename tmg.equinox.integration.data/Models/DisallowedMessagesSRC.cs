using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class DisallowedMessagesSRC: Entity
    {
        public string Description { get; set; }
        public string DisallowedExecutionCodeID { get; set; }
        public int ProcessGovernance1up { get; set; }
        public int Action { get; set; }
    }
}
