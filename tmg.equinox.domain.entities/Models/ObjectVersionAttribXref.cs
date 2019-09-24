using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class ObjectVersionAttribXref : Entity
    {
        public ObjectVersionAttribXref()
        {
            this.DateDataValues = new List<DateDataValue>();
            this.NumericDataValues = new List<NumericDataValue>();
            this.StringDataValues = new List<StringDataValue>();
        }

        public int ObjVerID { get; set; }
        public int VersionID { get; set; }
        public int OID { get; set; }
        public int AttrID { get; set; }
        public virtual ICollection<DateDataValue> DateDataValues { get; set; }
        public virtual ICollection<NumericDataValue> NumericDataValues { get; set; }
        public virtual ICollection<StringDataValue> StringDataValues { get; set; }
        public virtual Attribute Attribute { get; set; }
        public virtual ObjectDefinition ObjectDefinition { get; set; }
        public virtual ObjectVersion ObjectVersion { get; set; }
    }
}
