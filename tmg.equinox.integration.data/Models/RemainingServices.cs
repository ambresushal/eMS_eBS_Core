using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public class RemainingServices : Entity
    {
        public string ProductID { get; set; }
        public int ProcessGovernance1Up { get; set; }
        public string SESE_ID { get; set; }
        public string BenefitSet { get; set; }
        public string SESEHash { get; set; }
        public string SETRHash { get; set; }
        public string SESPHash { get; set; }
        public string SESERuleCategory { get; set; }
        public string NewSESERuleCategory { get; set; }
        public bool IsCreateRule { get; set; }
        public bool IsAltRule { get; set; }
        public bool IsCovered { get; set; }
        public string NewRule { get; set; }
        public string RuleDescription { get; set; }        
    }
}
