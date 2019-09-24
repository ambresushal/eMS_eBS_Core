using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class PBPSoftwareVersion : Entity
    {
        public int PBPSoftwareVersion1Up { get; set; }
        public string PBPSoftwareVersionName { get; set; }
        public string TestQaVesrion { get; set; }
        public bool IsLicenseVersion { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
