using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class ObjectVersion : Entity
    {
        public ObjectVersion()
        {
            this.ObjectTrees = new List<ObjectTree>();
            this.ObjectVersionAttribXrefs = new List<ObjectVersionAttribXref>();
            this.FormVersionObjectVersionMaps = new List<FormVersionObjectVersionMap>();
        }

        public int VersionID { get; set; }
        public string VersionName { get; set; }
        public Nullable<System.DateTime> EffectiveFrom { get; set; }
        public Nullable<System.DateTime> EffectiveTo { get; set; }
        public virtual ICollection<ObjectTree> ObjectTrees { get; set; }
        public virtual ICollection<ObjectVersionAttribXref> ObjectVersionAttribXrefs { get; set; }
        public virtual ICollection<FormVersionObjectVersionMap> FormVersionObjectVersionMaps { get; set; }
    }
}
