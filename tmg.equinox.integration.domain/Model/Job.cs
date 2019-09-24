using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tmg.equinox.integration.domain.Models
{
    public class Job
    {
        public string Id { get; set; }
        public string Run { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public DateTime StartDate { get; set; }
        public string StartTime { get; set; }
        public DateTime EndDate { get; set; }
        public string LastRunDateTime { get; set; }
        public string Location { get; set; }
        public string JobType { get; set; }
        public string Parameter { get; set; }
        public string Action { get; set; }
        public string Desc { get; set; }
        public int TriggerType { get; set; }
        public bool TriggerEnable { get; set; }
        public string WeekDaysList { get; set; }
        public Int16 Interval { get; set; }
    }
}