using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class UserActivity : Entity
    {
        public System.Guid EventID { get; set; }
        public string Category { get; set; }
        public string Event { get; set; }
        public Nullable<System.DateTime> TimeUtc { get; set; }
        public string UserName { get; set; }
        public string Host { get; set; }
        public string RequestUrl { get; set; }
        public string AppDomain { get; set; }
        public string UserAgent { get; set; }
        public Nullable<int> Priority { get; set; }
        public string Severity { get; set; }
        public Nullable<int> TenantID { get; set; }
        public string Message { get; set; }
    }
}
