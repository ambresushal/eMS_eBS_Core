using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.data.Models
{
    public class SEPYCompareResult : Entity
    {
        public string SEPY_PFX { get; set; } 
        public string SEPYPrefix2 { get; set; } 
        public string SESE_ID { get; set; }
        public DateTime? Date1 { get; set; }
        public string RuleDate1 { get; set; }
        public string AltRuleDate1 { get; set; }
        public string AltRuleConditionDate1 { get; set; }
        public DateTime? Date2 { get; set; }
        public string RuleDate2 { get; set; }
        public string AltRuleDate2 { get; set; }
        public string AltRuleConditionDate2 { get; set; }
        public int DifferentDueToDuplicateSet { get; set; }
        public int IsMismatch { get; set; }
        public string DuplicateSet { get; set; }
        public string Mismatch { get; set; }
        public bool CompareWithAnother { get; set; }
    }
}
