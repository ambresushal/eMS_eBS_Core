using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class FormInstanceRuleExecutionLog : Entity
    {
        public int RuleExecutionLoggerID { get; set; }
        public string SessionID { get; set; }
        public int FormInstanceID { get; set; }
        public int FolderID { get; set; }
        public int FolderVersionID { get; set; }
        public string FolderVersion { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }
        public bool IsParentRow { get; set; }
        public int ParentRowID { get; set; }
        public string OnEvent { get; set; }
        public int ElementID { get; set; }
        public string ElementLabel { get; set; } 
        public string ElementPath { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public int ImpactedElementID { get; set; }
        public string ImpactedElementLabel { get; set; }
        public string ImpactedElementPath { get; set; }
        public string ImpactDescription { get; set; }
        public string PropertyType { get; set; }
        public int RuleID { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public bool IsNewRecord { get; set; }
    }
}
