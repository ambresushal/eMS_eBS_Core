using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class FormInstanceDataMap : Entity
    {
        public int FormInstanceDataMapID { get; set; }
        public int FormInstanceID { get; set; }
        public int ObjectInstanceID { get; set; }
        public string FormData { get; set; }
        public string CompressJsonData { get; set; } 
        public virtual FormInstance FormInstance { get; set; }    
        public virtual ICollection<FormInstanceRepeaterDataMap> FormInstanceRepeaterDataMaps { get; set; }
    }
}
