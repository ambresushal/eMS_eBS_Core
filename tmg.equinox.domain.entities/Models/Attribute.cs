using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class Attribute : Entity
    {
        public Attribute()
        {
            this.ObjectVersionAttribXrefs = new List<ObjectVersionAttribXref>();
            this.RelationKeys = new List<RelationKey>();
            this.RelationKeys1 = new List<RelationKey>();
        }

        public int AttrID { get; set; }
        public string Name { get; set; }
        public string NameCamelcase { get; set; }
        public string AttrType { get; set; }
        public string Cardinality { get; set; }
        public Nullable<int> Length { get; set; }
        public Nullable<int> Precision { get; set; }
        public string EditRegex { get; set; }
        public string Formatter { get; set; }
        public Nullable<bool> Synthetic { get; set; }
        public string DefaultValue { get; set; }
        public int UIElementID { get; set; }
        public virtual ICollection<ObjectVersionAttribXref> ObjectVersionAttribXrefs { get; set; }
        public virtual ICollection<RelationKey> RelationKeys { get; set; }
        public virtual ICollection<RelationKey> RelationKeys1 { get; set; }
    }
}
