using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.masterListCascade
{
    public class ElementDocumentRuleViewModel
    {
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }
        public int ParentFormDesignVersionID { get; set; }
        public string TargetFieldPaths { get; set; }
        public string RuleJSON { get; set; }
        public bool RunOnMigration { get; set; }
    }
}
