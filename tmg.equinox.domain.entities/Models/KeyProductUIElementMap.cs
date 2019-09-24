using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class KeyProductUIElementMap : Entity
    {
        public int KeyProductUIElementMapID { get; set; }
        public int UIelementID { get; set; }
        public int ParentUIelementID { get; set; }
        public int MasterTemplateID { get; set; }
    }
}
