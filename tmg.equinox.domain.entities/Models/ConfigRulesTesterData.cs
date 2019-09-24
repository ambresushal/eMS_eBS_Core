using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class ConfigRulesTesterData : Entity
    {
        public int RuleTersterId { get; set; }
        public int FormDesignVersionId { get; set; }
        public int UIElementId { get; set; }
        public int RuleId { get; set; }
        public string TestData { get; set; }
        public DateTime? AddedDate { get; set; }
        public string AddedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
