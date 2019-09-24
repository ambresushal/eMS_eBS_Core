using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class DocumentRuleEventMap : Entity
    {
        public int DocumentRuleEventMapID { get; set; }
        public int DocumentRuleID { get; set; }
        public int DocumentRuleEventTypeID { get; set; }
    }
}
