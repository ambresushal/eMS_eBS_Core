using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class ObjectRelation : Entity
    {
        public ObjectRelation()
        {
            this.ObjectTrees = new List<ObjectTree>();
            this.RelationKeys = new List<RelationKey>();
        }

        public int RelationID { get; set; }
        public Nullable<int> RelatedObjectID { get; set; }
        public string RelationName { get; set; }
        public string RelationNameCamelcase { get; set; }
        public string Cardinality { get; set; }
        public virtual ObjectDefinition ObjectDefinition { get; set; }
        public virtual ICollection<ObjectTree> ObjectTrees { get; set; }
        public virtual ICollection<RelationKey> RelationKeys { get; set; }
    }
}
