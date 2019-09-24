using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class FormDesignHistory : Entity
    {
        public int FormDesignVersionId { get; set; }
        public string EnteredBy { get; set; }
        public Nullable<System.DateTime> EnteredDate { get; set; }
        public int TenantID { get; set; }
        public string Action { get; set; }
        public byte[] FormDesignVersionData { get; set; }
        public int FormDesignHistoryId { get; set; }
    }
}
