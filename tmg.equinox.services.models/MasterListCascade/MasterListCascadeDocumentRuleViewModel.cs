using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.viewmodels
{
    public class MasterListCascadeDocumentRuleViewModel
    {
        public string RuleJSON { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }
        public string TargetElementPath { get; set; }
        public string CompiledRuleJSON { get; set; }

        public int DocumentRuleTypeID { get; set; }

        public int SequenceNo { get; set; }

    }
}
