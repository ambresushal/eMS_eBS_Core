using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.RulesManager
{
    public class RuleHierarchyViewModel
    {
        public string SourceOperand { get; set; }
        public string ParentKey { get; set; }
        public int SourceElementID { get; set; }
        public string SourceOperandType { get; set; }
        public int RuleID { get; set; }
        public string RuleName { get; set; }
        public int TargetUIElementID { get; set; }
        public int Level { get; set; }
        public bool IsSourceATarget { get; set; }
        public string GroupID { get; set; }
        public string SourceElementPath { get; set; }
        public string SourceElementName { get; set; }
        public string TargetSectionPath { get; set; }
        public string TargetElementName { get; set; }
        public string RuleDescription { get; set; }
        public string TargetProperty { get; set; }
    }
}
