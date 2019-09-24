using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class FormVersionObjectVersionMap : Entity
    {
        public int FormVersionObjectVersionMap1 { get; set; }
        public int FormDesignVersionID { get; set; }
        public int ObjectVersionID { get; set; }
        public virtual ObjectVersion ObjectVersion { get; set; }
        public virtual FormDesignVersion FormDesignVersion { get; set; }
    }
}
