using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class ElementDocumentRule : Entity
    {
        public int ElementDocumentRuleID { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }
        public int? ParentFormDesignVersionID { get; set; }
        public string SourceField { get; set; }
        public string SourcePath { get; set; }
        public string TargetFields { get; set; }
        public string TargetPaths { get; set; }
        public string RuleJSON { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public bool RunOnMigration { get; set; }
    }
}
