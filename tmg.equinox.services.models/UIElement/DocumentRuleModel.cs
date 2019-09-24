using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.UIElement
{
    public class DocumentRuleExtModel : DocumentRuleModel
    {
        public string TargetUIElementName { get; set; }
        public string TargetUIElementLabel { get; set; }
        public string TargetSectionElementName { get; set; }
        public string TargetSectionGeneratedName { get; set; }
        public string TargetElementPathLabel { get; set; }
    }
    public class DocumentRuleModel
    {
        public int DocumentRuleID { get; set; }
        public string DisplayText { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int DocumentRuleTypeID { get; set; }
        public int DocumentRuleTargetTypeID { get; set; }
        public string RuleJSON { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }
        public int TargetUIElementID { get; set; }
        public string TargetElementPath { get; set; }
        public string CompiledRuleJSON { get; set; }
        public string ElementData { get; set; }
    }
}
