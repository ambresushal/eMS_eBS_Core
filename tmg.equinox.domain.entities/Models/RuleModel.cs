using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class RuleModel : Entity
    {
        public RuleModel()
        {

        }

        public int RuleID { get; set; }
        public string RuleCode { get; set; }
        public string RuleName { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int RuleTargetTypeID { get; set; }
        public string Section { get; set; }
        public string TargetElement { get; set; }
        public string SourceElement { get; set; }
        public string KeyFilter { get; set; }
    }
}
