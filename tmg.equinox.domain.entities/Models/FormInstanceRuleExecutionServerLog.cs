using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class FormInstanceRuleExecutionServerLog : Entity
    { 
        public int RowID { get; set; }
        public string SessionID { get; set; }
        public int FormInstanceID { get; set; }
        public string LoadedElement { get; set; }
        public bool IsParentRow { get; set; }
        public int ParentRowID { get; set; }
        public string OnEvent { get; set; }
        public int ElementID { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public int RuleID { get; set; }
        public int ImpactedElementID { get; set; }
        public bool Result { get; set; }
        public string LogFor { get; set; }

    }
}
