using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class StringDataValue : Entity
    {
        public int ValueID { get; set; }
        public Nullable<int> ObjVerID { get; set; }
        public string Value { get; set; }
        public long ObjInstanceID { get; set; }
        public Nullable<long> ParentObjInstanceID { get; set; }
        public Nullable<int> RowIDInfo { get; set; }
        public long RootObjInstanceID { get; set; }
        public virtual ObjectVersionAttribXref ObjectVersionAttribXref { get; set; }
    }
}
