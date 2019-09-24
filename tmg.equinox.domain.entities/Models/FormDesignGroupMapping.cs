using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class FormDesignGroupMapping : Entity
    {
        public int FormDesignGroupMappingID { get; set; }
        public int FormDesignGroupID { get; set; }
        public int FormID { get; set; }
        public Nullable<int> Sequence { get; set; }
        public Nullable<bool> AllowMultipleInstance { get; set; }
        public string AccessibleToRoles { get; set; }
        public virtual FormDesign FormDesign { get; set; }
        public virtual FormDesignGroup FormDesignGroup { get; set; }
    }
}
