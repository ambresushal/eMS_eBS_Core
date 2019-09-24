using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class FormInstanceHistory : Entity
    {
        public int FormInstanceID { get; set; }
        public string EnteredBy { get; set; }
        public Nullable<System.DateTime> EnteredDate { get; set; }
        public int TenantID { get; set; }
        public string Action { get; set; }
        public byte[] FormData { get; set; }
        public int FormInstanceHistoryId { get; set; }
    }
}
