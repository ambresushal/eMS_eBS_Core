using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.RulesManager
{
    public class RuleRowViewModel
    {
        public int RuleID { get; set; }
        public string RuleCode { get; set; }
        public string RuleName { get; set; }
        public int RuleTypeID { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Section { get; set; }
        public string SourceElement { get; set; }
        public string TargetElement { get; set; }
        public string Element { get; set; }
        public string KeyFilter { get; set; }
    }

    public class ElementRuleMap
    {
        public int RuleMapID { get; set; }
        public int UIElementID { get; set; }
        public int RuleID { get; set; }
        public List<TargetKeyFilter> TargetKeyFilter { get; set; }
    }

    public class TargetKeyFilter
    {
        public int UIElementID { get; set; }
        public string Label { get; set; }
        public string UIElementPath { get; set; }
        public string FilterValue { get; set; }
    }
}
