using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.data.Models
{
    public partial class BenefitHash : Entity
    {
        public string ProductID { get; set; }
        public int? ProcessGovernance1up { get; set; }
        public string SESE_ID { get; set; }
        public string BenefitSet { get; set; }
        public string SETRHash { get; set; }
        public string SESPHash { get; set; }
        public string SETRAltHash { get; set; }
        public string SESPAltHash { get; set; }
        public string IsCovered { get; set; }
        public string IsCoveredAlt { get; set; }
        public string SESERule { get; set; }
        public string SESEAltRule { get; set; }
        public string SESEHash { get; set; }
        public string SESEAltHash { get; set; }
        public string SESERuleCategory { get; set; }
        public string SESEAltRuleCategory { get; set; }
        public bool? HasHashCodeMatchRule { get; set; }
        public bool? HasHashCodeMatchAltRule { get; set; }
    }
}
