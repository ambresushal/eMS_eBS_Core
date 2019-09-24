using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class ObjectDefinition : Entity
    {
        public ObjectDefinition()
        {
            this.ObjectRelations = new List<ObjectRelation>();
            this.ObjectVersionAttribXrefs = new List<ObjectVersionAttribXref>();
            this.ObjectTrees = new List<ObjectTree>();
            this.ObjectTrees1 = new List<ObjectTree>();
        }

        public int OID { get; set; }
        public string ObjectName { get; set; }
        public int TenantID { get; set; }
        public Nullable<bool> Locked { get; set; }
        public virtual ICollection<ObjectRelation> ObjectRelations { get; set; }
        public virtual ICollection<ObjectVersionAttribXref> ObjectVersionAttribXrefs { get; set; }
        public virtual ICollection<ObjectTree> ObjectTrees { get; set; }
        public virtual ICollection<ObjectTree> ObjectTrees1 { get; set; }
    }
}
