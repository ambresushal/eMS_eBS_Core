using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class RelationKey : Entity
    {
        public int RelationKeyID { get; set; }
        public int RelationID { get; set; }
        public int LHSAttrID { get; set; }
        public int RHSAttrID { get; set; }
        public virtual Attribute Attribute { get; set; }
        public virtual Attribute Attribute1 { get; set; }
        public virtual ObjectRelation ObjectRelation { get; set; }
    }
}
