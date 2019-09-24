using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class DateDataValue : Entity
    {
        public int ValueID { get; set; }
        public int ObjVerID { get; set; }
        public System.DateTime Value { get; set; }
        public long ObjInstanceID { get; set; }
        public long ParentObjInstanceID { get; set; }
        public Nullable<int> RowIDInfo { get; set; }
        public long RootObjInstanceID { get; set; }
        public virtual ObjectVersionAttribXref ObjectVersionAttribXref { get; set; }
    }
}
