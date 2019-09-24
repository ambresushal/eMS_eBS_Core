using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class FormInstanceRepeaterDataMap : Entity
    {        
        
        public int FormInstanceRepeaterDataMapID { get; set; }
        public int FormInstanceDataMapID { get; set; }
        public int FormInstanceID { get; set; }
        public int RepeaterUIElementID { get; set; }
        public string SectionID { get; set; }
        public string FullName { get; set; }
        public string RepeaterData { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual FormInstanceDataMap FormInstanceDataMap { get; set; }
        public virtual FormInstance FormInstance { get; set; }
    }
}
