using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class FormDesignVersionUIElementMap : Entity
    {
        public int FormDesignVersionUIElementMapID { get; set; }
        public int FormDesignVersionID { get; set; }
        public int UIElementID { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public Nullable<System.DateTime> EffectiveDateOfRemoval { get; set; }
        public string Operation { get; set; }
        public virtual UIElement UIElement { get; set; }
        public virtual FormDesignVersion FormDesignVersion { get; set; }
    }
}
