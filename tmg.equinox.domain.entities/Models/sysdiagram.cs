using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class sysdiagram : Entity
    {
        public string name { get; set; }
        public int principal_id { get; set; }
        public int diagram_id { get; set; }
        public Nullable<int> version { get; set; }
        public byte[] definition { get; set; }
    }
}
