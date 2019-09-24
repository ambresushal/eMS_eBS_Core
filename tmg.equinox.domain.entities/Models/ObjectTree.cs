using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class ObjectTree : Entity
    {
        public int TreeID { get; set; }
        public Nullable<int> ParentOID { get; set; }
        public int RootOID { get; set; }
        public Nullable<int> RelationID { get; set; }
        public int VersionID { get; set; }
        public virtual ObjectDefinition ObjectDefinition { get; set; }
        public virtual ObjectDefinition ObjectDefinition1 { get; set; }
        public virtual ObjectRelation ObjectRelation { get; set; }
        public virtual ObjectVersion ObjectVersion { get; set; }
    }
}
