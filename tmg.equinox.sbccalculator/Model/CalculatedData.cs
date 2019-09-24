using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.sbccalculator.Model
{
    public class CalculatedDataModel
    {
        public string TreatmentType { get; set; }
        public string TotalCopayMemberCost { get; set; }
        public string RoundOffCopay { get; set; }
        public string TotalCoinsuranceMemberCost { get; set; }
        public string RoundOffCoinsurance { get; set; }
        public string TotalDeductibleMemberCost { get; set; }
        public string RoundOffDeductible { get; set; }
        public string OverallDeductible { get; set; }
        public string SpecialistCopay { get; set; }
        public string HospitalCopay { get; set; }
        public string OtherCopay { get; set; }
        public string TotalMemberCost { get; set; }
        public string FinalMemberCost { get; set; }
        public string RoundOffLimits { get; set; }
    }
}
