using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FolderVersion
{
    public class FormInstanceRuleExecutionLogViewModel
    {
        public int ID { get; set; }
        public string SessionID { get; set; }
        public int FormInstanceID { get; set; }
        public bool IsParentRow { get; set; }
        public int ParentRowID { get; set; }
        public string OnEvent { get; set; }
        public int ElementID { get; set; }
        public string ElementPath { get; set; }
        public string ElementLabel { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public int ImpactedElementID { get; set; }
        public string ImpactedElementPath { get; set; }
        public string ImpactedElementLabel { get; set; }
        public string ImpactDescription { get; set; }
        public string PropertyType { get; set; }
        public int RuleID { get; set; }
        public string FolderVersion { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public bool IsNewRecord { get; set; }
    }
}
