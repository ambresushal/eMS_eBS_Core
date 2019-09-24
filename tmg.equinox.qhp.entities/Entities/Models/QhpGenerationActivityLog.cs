using System;
using System.Collections.Generic;

namespace tmg.equinox.qhp.entities.Entities.Models
{
    public partial class QhpGenerationActivityLog
    {
        public int QhpActivityLogID { get; set; }
        public string Category { get; set; }
        public string Event { get; set; }
        public Nullable<System.DateTime> TimeUtc { get; set; }
        public string UserName { get; set; }
        public string Host { get; set; }
        public string URI { get; set; }
        public string Message { get; set; }
    }
}
