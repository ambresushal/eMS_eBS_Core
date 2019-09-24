using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.sbccalculator.Model
{
    public class BenefitReview
    {
        public string BenefitServiceCode { get; set; }
        public string BenefitCategory1 { get; set; }
        public string BenefitCategory2 { get; set; }
        public string BenefitCategory3 { get; set; }
        public string PlaceofService { get; set; }
        public string NetworkTier { get; set; }
        public string IndDeductible { get; set; }
        public string FamDeductible { get; set; }
        public string Copay { get; set; }
        public string Coinsurance { get; set; }
        public string Covered { get; set; }
    }

    public class RxBenefitReview
    {
        public string Network { get; set; }
        public string RxService { get; set; }
        public string RxTierType { get; set; }
        public string Covered { get; set; }
        public string Copay { get; set; }
        public string Coinsurance { get; set; }
        public string IndividualDeductible { get; set; }
    }
}
