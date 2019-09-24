using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class ShowActivitylog : Entity
    {
        public int Activity1up { get; set; }
        public int ProcessGovernance1up { get; set; }
        public int S { get; set; }
        public string Severity { get; set; }
        public string ActivityDetail { get; set; }
        public TimeSpan? TimeTaken { get; set; }
        public DateTime ActivityContextTime { get; set; }
        public int TenantID { get; set; }
        public string Resultset { get; set; }
        public string rs { get; set; }
        public string fields { get; set; }
        public string Indent { get; set; }
        public string O { get; set; }
        public string I { get; set; }
        public string C { get; set; }
    }
}
