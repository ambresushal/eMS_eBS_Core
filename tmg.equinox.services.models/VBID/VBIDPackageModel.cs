using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels
{
    public class VBIDPackageModel
    {
        public int PackageNumber{ get; set; }
        public bool IsCostSharing { get; set; }
        public bool IsAdditionalBenefits { get; set; }
        public bool IsRx { get; set; }
    }
}
