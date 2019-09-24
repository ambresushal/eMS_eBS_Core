using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class DocumentRuleEventType: Entity
    {
        public int DocumentRuleEventTypeID   { get; set; }
        public string DocumentRuleEventTypeCode { get; set; }
        public string DisplayText { get; set; }
    }
}
