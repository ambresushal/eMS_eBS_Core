using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.expressionrules.tester
{
    public class RuleDefinition
    {
        public RuleDefinition()
        {
            AddedBy = "SuperUser";
            AddedDate = DateTime.Now;
            UpdatedBy = null;
            UpdatedDate = DateTime.Now;
            IsActive = 1;
            DocumentRuleTypeID = 1;
            DocumentRuleTargetTypeID = 9;
        }

        public string DisplayText { get; set; }
        public string Description { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int IsActive { get; set; }
        public int DocumentRuleTypeID { get; set; }
        public int DocumentRuleTargetTypeID { get; set; }
        public string RuleJSON { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }
        public int TargetUIElementID { get; set; }
        public string TargetElementPath { get; set; }
        public string CompiledRuleJSON { get; set; }

        public string GetQuery()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("IF EXISTS (SELECT * FROM [UI].[DocumentRule] WHERE TargetUIElementID=" + this.TargetUIElementID + ")");
            sb.Append("\r\n\t");
            sb.Append("UPDATE [UI].[DocumentRule] SET RuleJSON = '" + this.RuleJSON.Replace("'", "''") + "' WHERE TargetUIElementID=" + this.TargetUIElementID);
            sb.Append("\r\n");
            sb.Append("ELSE");
            sb.Append("\r\n\t");
            sb.Append("INSERT INTO [UI].[DocumentRule] VALUES ('" + this.DisplayText + "' ,'" + this.DisplayText + "' ,'" + this.AddedBy + "' ,'" + this.AddedDate + "' ,'" + this.UpdatedBy + "' ,'" + this.UpdatedDate + "' ," + this.IsActive + " ," + this.DocumentRuleTypeID + " ," + this.DocumentRuleTargetTypeID + " ,'" + this.RuleJSON.Replace("'", "''") + "'," + this.FormDesignID + "," + this.FormDesignVersionID + "," + this.TargetUIElementID + ",'" + this.TargetElementPath + "','" + this.CompiledRuleJSON + "')");
            sb.Append("\r\n");
            sb.Append("GO");
            return sb.ToString();
        }

    }
}
