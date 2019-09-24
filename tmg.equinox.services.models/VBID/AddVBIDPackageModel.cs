using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels
{
    public class AddVBIDPackageModel
    {
        public int ReducedCostSharePackageRequired{ get; set; }
        public int ReducedCostSharePackageCurrent { get; set; }
        public int AdditionalBenefitPackageRequired { get; set; }
        public int AdditionalBenefitPackageCurrent { get; set; }
        public int RxPackageRequired { get; set; }
        public int RxPackageCurrent { get; set; }
    }
}
