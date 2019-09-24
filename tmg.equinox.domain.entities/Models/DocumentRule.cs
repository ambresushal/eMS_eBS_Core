using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class DocumentRule : Entity
    {
        public int DocumentRuleID { get; set; }
        public string DisplayText { get; set; }
        public string Description { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public int DocumentRuleTypeID { get; set; }
        public int DocumentRuleTargetTypeID { get; set; }
        public string RuleJSON { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }
        public int TargetUIElementID { get; set; }
        public string TargetElementPath { get; set; }
        public string CompiledRuleJSON { get; set; }
        public virtual DocumentRuleTargetType DocumentRuleTargetType { get; set; }
        public virtual DocumentRuleType DocumentRuleType { get; set; }
        public virtual FormDesign FormDesign { get; set; }
        public virtual FormDesignVersion FormDesignVersion { get; set; }
        public virtual UIElement UIElement { get; set; }
    }
}
