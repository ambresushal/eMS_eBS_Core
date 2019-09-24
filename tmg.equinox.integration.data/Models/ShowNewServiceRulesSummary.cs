using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class ShowNewServiceRulesSummary : Entity
    {
        public string NewRule { get; set; }
        public string RuleDescription { get; set; }
    }
}